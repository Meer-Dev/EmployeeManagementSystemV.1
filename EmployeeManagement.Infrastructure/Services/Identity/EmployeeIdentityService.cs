using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.Infrastructure.Services.Identity;

public sealed class EmployeeIdentityService(UserManager<Employee> userManager) : IEmployeeIdentityService
{
    private readonly UserManager<Employee> _userManager = userManager;

    public async Task<int> CreateEmployeeAsync(Employee employee, string password, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = await _userManager.CreateAsync(employee, password);
        if (!result.Succeeded)
        {
            var message = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(message);
        }

        return employee.Id;
    } //here what we are doing is we are creating a new employee using the UserManager's CreateAsync method. We pass in the employee object and the password. If the creation is successful, we return the employee's Id. If it fails, we throw an exception with the error messages.
}

