// Application Layer: Employees/Queries/GetAdmins
using EmployeeManagement.Application.Employees.Dtos;
using MediatR;
using System.Collections.Generic;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees
{
    public record GetAdminsQuery() : IRequest<List<EmployeeDto>>;
}
