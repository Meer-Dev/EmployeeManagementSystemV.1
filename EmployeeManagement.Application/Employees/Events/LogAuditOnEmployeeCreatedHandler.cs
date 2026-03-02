using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Employees.Events;

public class LogAuditOnEmployeeCreatedHandler(ILogger<LogAuditOnEmployeeCreatedHandler> logger)
    : INotificationHandler<EmployeeCreatedEvent>
{
    public Task Handle(EmployeeCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "AUDIT: Employee created. EmployeeId={EmployeeId}, Email={Email}, Timestamp={Timestamp}",
            notification.EmployeeId,
            notification.Email,
            DateTimeOffset.UtcNow);

        return Task.CompletedTask;
    }
}