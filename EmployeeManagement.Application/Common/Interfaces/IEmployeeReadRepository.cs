using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.Employees.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Application.Common.Interfaces
{
    public interface IEmployeeReadRepository
    {
        // ✅ Returns all admins
        Task<List<EmployeeDto>> GetAdminsAsync();

        // ✅ Get employee by Id
        Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<int> GetActiveEmployeeCountAsync(CancellationToken ct = default);


        // ✅ Get employees with pagination & filters
        Task<PagedResult<EmployeeDto>> GetPagedAsync(
            int? id = null,
            string? email = null,
            string? firstName = null,
            string? lastName = null,
            string? department = null,
            string? sortBy = "id",
            bool ascending = true,
            int pageSize = 20,
            int? pageNumber = null,
            int? lastSeenId = null,
            CancellationToken cancellationToken = default);
    }
}