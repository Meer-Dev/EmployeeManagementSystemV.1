namespace EmployeeManagement.Application.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler(IUnitOfWork context) : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IUnitOfWork _context = context;

    public async Task Handle(
        DeleteEmployeeCommand request,
        CancellationToken cancellationToken) 
    {
        var employee = (await _context.GetEmployeeByIdAsync(request.Id, cancellationToken) ??
            throw new InvalidOperationException($"Employee with ID {request.Id} not found")) ?? throw new System.Collections.Generic.KeyNotFoundException("Employee not found.");
        if (!employee.IsActive)
            throw new InvalidOperationException("Employee already removed.");

        employee.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
