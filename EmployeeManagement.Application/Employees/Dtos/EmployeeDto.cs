namespace EmployeeManagement.Application.Employees.Dtos;

public record EmployeeDto(
    int Id,
    string Pass,
    string FirstName,
    string LastName,
    string Email,
    string Department,
    bool IsActive
);

//employee dto is necessary in query handlers to transfer employee data between application layers which it does by encapsulating employee properties in a simple, immutable object