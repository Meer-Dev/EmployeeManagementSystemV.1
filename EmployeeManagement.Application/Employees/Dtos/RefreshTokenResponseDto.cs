namespace EmployeeManagement.Application.Employees.Dtos;

public class RefreshTokenResponseDto
{
    public RefreshTokenResponseDto(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}