

namespace EmployeeManagement.Application.Employees.Commands.PatchEmployee;

public record PatchEmployeeCommand(
int Id,
    string? FirstName = null,
    string? LastName = null,
    string? Email = null,
    string? Department = null,
    bool? IsActive = true
    ) : IRequest;
