namespace EmployeeManagement.Application.Employees.Dtos;
public record LoginRequestDto(
    string Email,
    string Password
);