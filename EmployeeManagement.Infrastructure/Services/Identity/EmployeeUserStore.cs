using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Services.Identity;

/// <summary>
/// Minimal ASP.NET Core Identity user store backed by the existing Employees table.
/// This lets us use UserManager (hashing, validation) without switching to IdentityDbContext/AspNetUsers tables.
/// </summary>
public sealed class EmployeeUserStore(AppDbContext context)
    : IUserStore<Employee>,
      IUserPasswordStore<Employee>,
      IUserEmailStore<Employee>
{
    private readonly AppDbContext _context = context;

    public void Dispose() { }

    public async Task<IdentityResult> CreateAsync(Employee user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Employees.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(Employee user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Employees.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(Employee user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _context.Employees.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public Task<Employee?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!int.TryParse(userId, out var id)) return Task.FromResult<Employee?>(null);
        return _context.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<Employee?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // We treat Email as the username.
        // Avoid wrapping column in UPPER() so SQL Server can use the Email index (typical CI collations will still match).
        return _context.Employees.FirstOrDefaultAsync(e => e.Email == normalizedUserName, cancellationToken);
    }

    public Task<string?> GetNormalizedUserNameAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.Email.ToUpperInvariant());

    public Task<string?> GetUserNameAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.Email);

    public Task SetNormalizedUserNameAsync(Employee user, string? normalizedName, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task SetUserNameAsync(Employee user, string? userName, CancellationToken cancellationToken)
    {
        // Email has a private setter in the domain entity; update via method.
        if (!string.IsNullOrWhiteSpace(userName))
            user.UpdateEmail(userName);
        return Task.CompletedTask;
    }

    public Task<string> GetUserIdAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult(user.Id.ToString());

    public Task SetPasswordHashAsync(Employee user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.SetPassword(passwordHash ?? string.Empty);
        return Task.CompletedTask;
    }

    public Task<string?> GetPasswordHashAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.PasswordHash);

    public Task<bool> HasPasswordAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

    public Task SetEmailAsync(Employee user, string? email, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(email))
            user.UpdateEmail(email);
        return Task.CompletedTask;
    }

    public Task<string?> GetEmailAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.Email);

    public Task<bool> GetEmailConfirmedAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult(true);

    public Task SetEmailConfirmedAsync(Employee user, bool confirmed, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public Task<Employee?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        // Avoid wrapping column in UPPER() so SQL Server can use the Email index (typical CI collations will still match).
        return _context.Employees.FirstOrDefaultAsync(e => e.Email == normalizedEmail, cancellationToken);
    }

    public Task<string?> GetNormalizedEmailAsync(Employee user, CancellationToken cancellationToken)
        => Task.FromResult<string?>(user.Email.ToUpperInvariant());

    public Task SetNormalizedEmailAsync(Employee user, string? normalizedEmail, CancellationToken cancellationToken)
        => Task.CompletedTask;
}

