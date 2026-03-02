using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EmployeeManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("SlidingWindow")]

public class AuthController(
    IUnitOfWork unitOfWork,
    IJwtService jwtService,
    UserManager<Employee> userManager,
    ILogger<AuthController> logger) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IJwtService _jwtService = jwtService;
    private readonly UserManager<Employee> _userManager = userManager;
    private readonly ILogger<AuthController> _logger = logger;

    private const string RefreshTokenCookieName = "refreshToken";
    private const int RefreshTokenExpiryDays = 7;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) != null)
            return BadRequest("Email already exists");

        var employee = new Employee(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Department,
            string.Empty,
            request.Role ?? "User");

        var createResult = await _userManager.CreateAsync(employee, request.Password);
        if (!createResult.Succeeded)
        {
            var message = string.Join("; ", createResult.Errors.Select(e => e.Description));
            return BadRequest(message);
        }

        var (accessToken, refreshToken) = _jwtService.GenerateTokenPair(employee);
        await StoreRefreshTokenAsync(employee.Id, refreshToken);

        // Set httpOnly cookie with refresh token
        SetRefreshTokenCookie(refreshToken);

        _logger.LogInformation("Employee {EmployeeId} registered successfully", employee.Id);

        return Ok(new AuthResponseDto(
            employee.Id,
            accessToken,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Role));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        var employee = await _userManager.FindByEmailAsync(request.Email);
        if (employee == null)
        {
            _logger.LogWarning("Login attempt for non-existent email: {Email}", request.Email);
            return Unauthorized("Invalid credentials");
        }

        var ok = await _userManager.CheckPasswordAsync(employee, request.Password);
        if (!ok)
        {
            _logger.LogWarning("Failed login attempt for user: {Email}", request.Email);
            return Unauthorized("Invalid credentials");
        }

        var (accessToken, refreshToken) = _jwtService.GenerateTokenPair(employee);
        await StoreRefreshTokenAsync(employee.Id, refreshToken);

        // Set httpOnly cookie with refresh token
        SetRefreshTokenCookie(refreshToken);

        _logger.LogInformation("Employee {EmployeeId} logged in successfully", employee.Id);

        return Ok(new AuthResponseDto(
            employee.Id,
            accessToken,
            employee.FirstName,
            employee.LastName,
            employee.Email,
            employee.Role));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        // Try to get refresh token from cookie first, then fall back to request body
        var refreshToken = Request.Cookies[RefreshTokenCookieName] ?? request.RefreshToken;

        if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest("Access token and refresh token are required");

        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            _logger.LogWarning("Invalid token principal");
            return Unauthorized("Invalid token");
        }

        var employeeIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out var employeeId))
        {
            _logger.LogWarning("Invalid token claims");
            return Unauthorized("Invalid token claims");
        }

        var storedTokens = await _unitOfWork.RefreshTokens.GetAllAsync();
        var storedToken = storedTokens
            .FirstOrDefault(rt => rt.EmployeeId == employeeId && rt.Token == refreshToken);

        if (storedToken == null || !storedToken.IsValid)
        {
            _logger.LogWarning("Invalid or expired refresh token for employee {EmployeeId}", employeeId);
            return Unauthorized("Invalid or expired refresh token");
        }

        var employee = await _userManager.FindByIdAsync(employeeId.ToString());
        if (employee == null)
        {
            _logger.LogWarning("Employee {EmployeeId} not found", employeeId);
            return Unauthorized("Employee not found");
        }

        var (newAccessToken, newRefreshToken) = _jwtService.GenerateTokenPair(employee);
        
        // Revoke old refresh token and store new one in a transaction
        await _unitOfWork.ExecuteTransactionAsync(async () =>
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshTokens.UpdateAsync(storedToken);
            await StoreRefreshTokenAsync(employeeId, newRefreshToken);
            return true;
        });

        // Update httpOnly cookie with new refresh token
        SetRefreshTokenCookie(newRefreshToken);

        _logger.LogInformation("Refresh token used for employee {EmployeeId}", employeeId);

        return Ok(new RefreshTokenResponseDto(newAccessToken, newRefreshToken));
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        // Clear the refresh token cookie
        ClearRefreshTokenCookie();

        // Optionally revoke the refresh token from the database
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var storedTokens = await _unitOfWork.RefreshTokens.GetAllAsync();
            var storedToken = storedTokens.FirstOrDefault(rt => rt.Token == refreshToken);
            if (storedToken != null)
            {
                storedToken.RevokedAt = DateTime.UtcNow;
                await _unitOfWork.RefreshTokens.UpdateAsync(storedToken);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        _logger.LogInformation("User logged out successfully");
        return Ok(new { message = "Logged out successfully" });
    }

    private async Task StoreRefreshTokenAsync(int employeeId, string refreshToken)
    {
        var token = new RefreshToken
        {
            EmployeeId = employeeId,
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
        };

        await _unitOfWork.RefreshTokens.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(RefreshTokenExpiryDays)
        };

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }

    private void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookieName);
    }
}
