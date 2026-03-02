using EmployeeManagement.Domain.Entities;
using System.Security.Claims;

namespace EmployeeManagement.Infrastructure.Services.Jwt;

public interface IJwtService
{
    string GenerateToken(Employee employee);
    (string accessToken, string refreshToken) GenerateTokenPair(Employee employee);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
