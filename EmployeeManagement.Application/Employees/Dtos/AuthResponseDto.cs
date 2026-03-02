
namespace EmployeeManagement.Application.Employees.Dtos;

public record AuthResponseDto(
    int Id,
    string Token,
    string FirstName,
    string LastName,
    string Email,
    string Role
);