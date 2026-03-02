using EmployeeManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace EmployeeManagement.Infrastructure.Services.Cache;

public class RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly ILogger<RedisCacheService> _logger = logger;

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>((string)value!);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET failed for key '{Key}'. Falling back to source.", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, (Expiration)expiry);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET failed for key '{Key}'.", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _db.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis DELETE failed for key '{Key}'.", key);
        }
    }
}

//explanation (with real world scenario and example) : 
//In a real-world scenario, let's say we have a web application that displays employee profiles. When a user views a profile, we can cache the profile data in Redis to improve performance. The first time the profile is requested, it will be fetched from the database and stored in Redis. 
//Subsequent requests for the same profile can be served directly from the cache, reducing database load and improving response times.
//For example, when a user views the profile of an employee with ID 123, we can use the RedisCacheService to store the profile data with the key "employee:123". If another user requests the same profile, the service will retrieve it from Redis instead of hitting the database again. redis cache is saved in memory and is very fast, so it can significantly improve the performance of read-heavy operations.
//Additionally, by setting an expiration time on the cache entries, we can ensure that stale data is eventually removed and refreshed from the database as needed. in our scenario, we might set the cache to expire after 5 minutes, so if the employee's profile is updated in the database, the cache will eventually reflect those changes after expiration. This approach helps us balance performance with data freshness.
//We use generics like <T> to allow the cache service to work with any type of data, making it flexible and reusable across different parts of the application. The use of try-catch blocks ensures that if Redis is unavailable for any reason, the application can gracefully fall back to fetching data from the source without crashing, while also logging the issue for later investigation.