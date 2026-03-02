

namespace EmployeeManagement.Application.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler(IUnitOfWork context) : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IUnitOfWork _context = context;

    public async Task Handle(
        UpdateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _context.GetEmployeeByIdAsync(request.Id, cancellationToken) ??
            throw new InvalidOperationException($"Employee with ID {request.Id} not found");

        employee.Update(request.FirstName, request.LastName, request.Email, request.Department, request.IsActive);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
