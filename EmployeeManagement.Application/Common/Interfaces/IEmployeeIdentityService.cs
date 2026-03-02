using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IEmployeeIdentityService
{
    Task<int> CreateEmployeeAsync(Employee employee, string password, CancellationToken cancellationToken);
}

