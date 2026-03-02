using MediatR;

namespace EmployeeManagement.Application.Employees.Events;

public record EmployeeCreatedEvent(int EmployeeId, string Email) : INotification;