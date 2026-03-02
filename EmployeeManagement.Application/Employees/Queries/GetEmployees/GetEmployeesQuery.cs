using EmployeeManagement.Application.Common;
using MediatR;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQuery : IRequest<PagedResult<EmployeeDto>>
{
    // Filters
    public int? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Department { get; set; }

    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? LastSeenId { get; set; }

    // Sorting
    public string? SortBy { get; set; } = "id";
    public bool Ascending { get; set; } = true;
}
