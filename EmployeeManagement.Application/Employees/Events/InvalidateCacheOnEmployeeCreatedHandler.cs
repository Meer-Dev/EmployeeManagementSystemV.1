using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Employees.Events;

public class InvalidateCacheOnEmployeeCreatedHandler(
    ICacheService cache,
    ILogger<InvalidateCacheOnEmployeeCreatedHandler> logger)
    : INotificationHandler<EmployeeCreatedEvent>
{
    public async Task Handle(EmployeeCreatedEvent notification, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync("employees:all");
        logger.LogInformation(
            "Cache invalidated for 'employees:all' after Employee {EmployeeId} was created.",
            notification.EmployeeId);
    }
}