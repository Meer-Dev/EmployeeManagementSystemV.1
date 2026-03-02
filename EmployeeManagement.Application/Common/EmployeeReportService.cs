// ✅ EmployeeReportService.cs - Option A
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using System.Linq.Expressions;

public class EmployeeReportService(IUnitOfWork unitOfWork) : IEmployeeReportService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task GenerateDailyReport()
    {
        // ✅ Use IRepository<T>.FindAsync with Expression
        var activeEmployees = await _unitOfWork.Employees.FindAsync(
            e => e.IsActive  // Lambda automatically converts to Expression<Func<Employee, bool>>
        );

        Console.WriteLine($"Daily report: {activeEmployees.Count()} active employees.");
    }
}
//Right now its underutilized we can utilize it properly by using the FindAsync method to filter active employees directly in the database query, instead of fetching all employees and filtering in memory. This way we leverage the power of expressions and improve performance. IQueryable does the same thing but it returns an IQueryable which allows for further composition of queries before execution, while FindAsync executes immediately and returns a list. In this case, since we just want to get the active employees for the report, using FindAsync with an expression is more straightforward and efficient.