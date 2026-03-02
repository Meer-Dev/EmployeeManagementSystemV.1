using EmployeeManagement.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EmployeeManagement.Infrastructure.Services.Jwt;

public class JwtService(IOptions<JwtOptions> jwtOptions) : IJwtService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value ?? 
        throw new ArgumentNullException(nameof(jwtOptions), "JwtOptions cannot be null");

    private byte[] GetSecretKey()
    {
        if (string.IsNullOrWhiteSpace(_jwtOptions.Key))
            throw new InvalidOperationException("JWT Key is not configured");

        return Encoding.UTF8.GetBytes(_jwtOptions.Key);
    }

    public string GenerateToken(Employee employee)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = GetSecretKey();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role),
                new Claim("FullName", $"{employee.FirstName} {employee.LastName}"),
                new Claim("Department", employee.Department)  // ✅ Add this
            }),
            Expires = DateTime.UtcNow.AddMinutes(
                int.TryParse(_jwtOptions.ExpiryMinutes, out var minutes) ? minutes : 15),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public (string accessToken, string refreshToken) GenerateTokenPair(Employee employee)
    {
        var accessToken = GenerateToken(employee);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        return (accessToken, refreshToken);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = GetSecretKey();

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = false // Allow expired tokens to be validated
            }, out SecurityToken securityToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
