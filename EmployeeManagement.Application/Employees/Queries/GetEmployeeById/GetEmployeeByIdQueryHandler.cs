namespace EmployeeManagement.Application.Employees.Queries.GetEmployeeById;
public class GetEmployeeByIdQueryHandler(IEmployeeReadRepository repository)
        : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    private readonly IEmployeeReadRepository _repository = repository;

    public async Task<EmployeeDto> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var employee = await _repository
            .GetByIdAsync(request.Id, cancellationToken) ?? throw new System.Collections.Generic.KeyNotFoundException(
                $"Employee with ID {request.Id} not found.");
        return employee;
    }
}
