using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees
{
    public class GetAdminsQueryHandler(IEmployeeReadRepository readRepo)
        : IRequestHandler<GetAdminsQuery, List<EmployeeDto>>
    {
        public async Task<List<EmployeeDto>> Handle(
            GetAdminsQuery request,
            CancellationToken cancellationToken)
        {
            return await readRepo.GetAdminsAsync();
        }
    }
}