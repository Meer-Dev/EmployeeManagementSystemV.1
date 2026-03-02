using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.Common.Interfaces;
using MediatR;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(
    IEmployeeReadRepository repository,
    ICacheService cache)
    : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    // ✅ No explicit constructor needed - primary constructor handles DI
    // ✅ No private fields needed - use parameters directly

    // Cache key encodes the query params so different filters/pages get their own cache entry
    private static string BuildCacheKey(GetEmployeesQuery q) =>
        $"employees:page:{q.PageNumber}:size:{q.PageSize}:sort:{q.SortBy}:asc:{q.Ascending}" +
        $":id:{q.Id}:email:{q.Email}:first:{q.FirstName}:last:{q.LastName}:dept:{q.Department}:cursor:{q.LastSeenId}";

    public async Task<PagedResult<EmployeeDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = BuildCacheKey(request);

        // Try cache first
        var cached = await cache.GetAsync<PagedResult<EmployeeDto>>(cacheKey);
        if (cached is not null)
            return cached;

        // Cache miss — fetch from DB
        var result = await repository.GetPagedAsync(
            request.Id,
            request.Email,
            request.FirstName,
            request.LastName,
            request.Department,
            request.SortBy,
            request.Ascending,
            request.PageSize,
            request.PageNumber,
            request.LastSeenId,
            cancellationToken
        );

        // Store in cache for 5 minutes
        await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
    }
}