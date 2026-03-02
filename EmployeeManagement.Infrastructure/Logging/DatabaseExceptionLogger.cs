using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;

namespace EmployeeManagement.Infrastructure.Logging;

public class DatabaseExceptionLogger(AppDbContext context) : IExceptionLogger
{
    private readonly AppDbContext _context = context;

    public async Task LogAsync(
        Exception exception,
        string? path,
        string? method,
        int? statusCode,
        CancellationToken cancellationToken = default)
    {
        var log = new ExceptionLog
        {
            ExceptionType = exception.GetType().FullName ?? exception.GetType().Name,
            Message = exception.Message,
            StackTrace = exception.StackTrace,
            Path = path,
            Method = method,
            StatusCode = statusCode,
            CreatedAtUtc = DateTime.UtcNow
        };

        _context.ExceptionLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

