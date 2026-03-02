
namespace EmployeeManagement.Application.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(int Id) : IRequest<EmployeeDto>;
