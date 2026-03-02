


namespace EmployeeManagement.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email,
    string Department,
    string Password,
    string Role // default role is User
) : IRequest<int>;
