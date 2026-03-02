namespace EmployeeManagement.Application.Employees.Dtos;

public record RegisterRequestDto(
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Password,
    string Role = "User" // default role is User
);
