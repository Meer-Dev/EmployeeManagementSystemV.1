namespace EmployeeManagement.Infrastructure.Services.Jwt;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string ExpiryMinutes { get; set; } = "60";
}
