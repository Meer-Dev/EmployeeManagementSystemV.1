

namespace EmployeeManagement.Application.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(
int Id,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    bool IsActive) : IRequest;
