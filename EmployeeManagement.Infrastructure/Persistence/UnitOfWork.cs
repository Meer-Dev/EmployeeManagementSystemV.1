using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence;

// ✅ UnitOfWork.cs - Remove explicit interface implementations that throw
// ✅ UnitOfWork.cs - Wraps DbContext, implements IUnitOfWork
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private IRepository<Employee>? _employees;
    private IRepository<RefreshToken>? _refreshTokens;

    public IRepository<Employee> Employees =>
        _employees ??= new Repository<Employee>(_context);

    public IRepository<RefreshToken> RefreshTokens =>
        _refreshTokens ??= new Repository<RefreshToken>(_context);

    // ✅ FIXED: Return Employee?, not object?
    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        => await _context.Employees.FindAsync([id], cancellationToken);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public async Task<T> ExecuteTransactionAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await operation();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}