using System;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IExceptionLogger
{
    Task LogAsync(
        Exception exception,
        string? path,
        string? method,
        int? statusCode,
        CancellationToken cancellationToken = default);
}

