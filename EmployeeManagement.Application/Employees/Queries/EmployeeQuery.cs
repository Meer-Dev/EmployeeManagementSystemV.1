using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;

public class EmployeeQuery
{
    private readonly IEmployeeReadRepository _repo;

    public EmployeeQuery(IEmployeeReadRepository repo)
    {
        _repo = repo;
    }

    // GraphQL field: employees
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<List<EmployeeDto>> GetEmployees()
    {
        // Example: get all employees (adjust as needed)
        var paged = await _repo.GetPagedAsync(pageSize: 100);
        return (List<EmployeeDto>)paged.Items;
    }
}