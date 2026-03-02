using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Events;
using EmployeeManagement.Domain.Entities;
using MediatR;

namespace EmployeeManagement.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler(
    IEmployeeIdentityService identity,
    IPublisher publisher) : IRequestHandler<CreateEmployeeCommand, int>
{
    public async Task<int> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        Employee employee = new(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Department,
            passwordHash: string.Empty,
            request.Role
        );

        var id = await identity.CreateEmployeeAsync(employee, request.Password, cancellationToken);

        await publisher.Publish(new EmployeeCreatedEvent(id, request.Email), cancellationToken);

        return id;
    }
}