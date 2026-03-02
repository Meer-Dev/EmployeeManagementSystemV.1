// ✅ FIXED: IUnitOfWork.cs
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IUnitOfWork : IAsyncDisposable
{
    // Repositories for generic CRUD
    IRepository<Employee> Employees { get; }
    IRepository<RefreshToken> RefreshTokens { get; }

    // Specific methods for Employee (if needed)
    Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);

    // Transaction & persistence
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<T> ExecuteTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default);
}