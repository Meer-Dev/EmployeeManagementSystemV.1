using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Repositories;

public class EmployeeReadRepository : IEmployeeReadRepository
{
    private readonly AppDbContext _context;

    public EmployeeReadRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetActiveEmployeeCountAsync(CancellationToken ct = default)
    => await _context.Employees
        .AsNoTracking()
        .CountAsync(e => e.IsActive, ct);

    public async Task<List<EmployeeDto>> GetAdminsAsync()
    {
        return await _context.Employees
            .Where(e => e.Role == "Admin")
            .Select(e => new EmployeeDto(
                e.Id,
                e.PasswordHash,
                e.FirstName,
                e.LastName,
                e.Email,
                e.Department,
                e.IsActive
            ))
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _context.Employees
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto(
                e.Id,
                e.PasswordHash,
                e.FirstName,
                e.LastName,
                e.Email,
                e.Department,
                e.IsActive
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<EmployeeDto>> GetPagedAsync(
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
        CancellationToken cancellationToken = default)
    {
        if (pageSize <= 0)
            throw new InvalidOperationException("PageSize must be greater than zero.");

        if (pageNumber.HasValue && pageNumber <= 0)
            throw new InvalidOperationException("PageNumber must be greater than zero.");

        var baseQuery = _context.Employees.AsNoTracking().AsQueryable();

        // -------------------------------
        // Count how many filters are provided
        // -------------------------------
        int filtersUsed = 0;
        if (id.HasValue) filtersUsed++;
        if (!string.IsNullOrWhiteSpace(email)) filtersUsed++;
        if (!string.IsNullOrWhiteSpace(firstName)) filtersUsed++;
        if (!string.IsNullOrWhiteSpace(lastName)) filtersUsed++;
        if (!string.IsNullOrWhiteSpace(department)) filtersUsed++;

        if (filtersUsed > 1)
            throw new InvalidOperationException("Only one search parameter is allowed.");
        // -------------------------------
        // Apply filter
        // -------------------------------
        if (id.HasValue)
            baseQuery = baseQuery.Where(e => e.Id == id.Value);
        else if (!string.IsNullOrWhiteSpace(email))
            baseQuery = baseQuery.Where(e => e.Email == email);
        else if (!string.IsNullOrWhiteSpace(firstName))
            baseQuery = baseQuery.Where(e => e.FirstName.StartsWith(firstName));
        else if (!string.IsNullOrWhiteSpace(lastName))
            baseQuery = baseQuery.Where(e => e.LastName.StartsWith(lastName));
        else if (!string.IsNullOrWhiteSpace(department))
            baseQuery = baseQuery.Where(e => e.Department.StartsWith(department));

        // -------------------------------
        // Total count (for offset pagination)
        // -------------------------------
        int totalCount = 0;
        if (!lastSeenId.HasValue)
            totalCount = await baseQuery.CountAsync(cancellationToken);

        // -------------------------------
        // Sorting
        // -------------------------------
        var sortedQuery = (sortBy?.ToLower()) switch
        {
            "firstname" => ascending
                ? baseQuery.OrderBy(e => e.FirstName).ThenBy(e => e.Id)
                : baseQuery.OrderByDescending(e => e.FirstName).ThenByDescending(e => e.Id),

            "lastname" => ascending
                ? baseQuery.OrderBy(e => e.LastName).ThenBy(e => e.Id)
                : baseQuery.OrderByDescending(e => e.LastName).ThenByDescending(e => e.Id),

            "email" => ascending
                ? baseQuery.OrderBy(e => e.Email).ThenBy(e => e.Id)
                : baseQuery.OrderByDescending(e => e.Email).ThenByDescending(e => e.Id),

            "department" => ascending
                ? baseQuery.OrderBy(e => e.Department).ThenBy(e => e.Id)
                : baseQuery.OrderByDescending(e => e.Department).ThenByDescending(e => e.Id),

            _ => ascending
                ? baseQuery.OrderBy(e => e.Id)
                : baseQuery.OrderByDescending(e => e.Id)
        };

        // -------------------------------
        // Keyset Pagination Validation
        // -------------------------------
        if (lastSeenId.HasValue && sortBy?.ToLower() != "id")
            throw new InvalidOperationException("Keyset pagination requires sorting by Id.");

        // -------------------------------
        // Pagination
        // -------------------------------
        IQueryable<Employee> pagedQuery = lastSeenId.HasValue
            ? sortedQuery.Where(e => e.Id > lastSeenId.Value)
            : pageNumber.HasValue
                ? sortedQuery.Skip((pageNumber.Value - 1) * pageSize)
                : sortedQuery;

        // -------------------------------
        // Projection
        // -------------------------------
        var items = await pagedQuery
            .Take(pageSize)
            .Select(e => new EmployeeDto(
                e.Id,
                e.PasswordHash,
                e.FirstName,
                e.LastName,
                e.Email,
                e.Department,
                e.IsActive
            ))
            .ToListAsync(cancellationToken);

        int? newLastSeenId = items.Count > 0 ? items.Last().Id : null;

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = totalCount,
            LastSeenId = newLastSeenId
        };
    }
}
