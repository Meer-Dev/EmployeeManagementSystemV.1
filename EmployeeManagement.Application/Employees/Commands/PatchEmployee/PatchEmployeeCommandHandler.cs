

namespace EmployeeManagement.Application.Employees.Commands.PatchEmployee;

public class PatchEmployeeCommandHandler(IUnitOfWork context) : IRequestHandler<PatchEmployeeCommand>
{
    private readonly IUnitOfWork _context = context;

    public async Task Handle(
        PatchEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = await _context.GetEmployeeByIdAsync(request.Id, cancellationToken) ??
            throw new InvalidOperationException($"Employee with ID {request.Id} not found");

        if (!string.IsNullOrEmpty(request.FirstName))
        {
            employee.UpdateFirstName(request.FirstName);
        }



        if (!string.IsNullOrEmpty(request.LastName))
        {
            employee.UpdateLastName(request.LastName);
        }



        if (!string.IsNullOrEmpty(request.Email))
        {
            employee.UpdateEmail(request.Email);
        }




        if (!string.IsNullOrEmpty(request.Department))
        {
            employee.UpdateDepartment(request.Department);
        }



        if (request.IsActive.HasValue)
        {
            employee.UpdateIsActive(request.IsActive.Value);
        }


        await _context.SaveChangesAsync(cancellationToken);
    }
}
