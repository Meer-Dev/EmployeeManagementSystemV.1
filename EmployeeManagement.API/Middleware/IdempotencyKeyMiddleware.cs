using System.Security.Cryptography;
using System.Text;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.API.Middleware;

/// <summary>
/// Middleware for handling idempotency keys to prevent duplicate request processing.
/// Stores results of completed requests and returns cached responses for retries.
/// </summary>
public class IdempotencyKeyMiddleware(
    RequestDelegate next,
    ILogger<IdempotencyKeyMiddleware> logger,
    IServiceProvider serviceProvider)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<IdempotencyKeyMiddleware> _logger = logger;
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    private const string IdempotencyKeyHeader = "Idempotency-Key";
    private const string IdempotencyStatusHeader = "Idempotency-Status";

    public async Task InvokeAsync(HttpContext context)
    {
        // Only apply idempotency to POST, PUT, PATCH, DELETE requests
        if (!IsIdempotentRequest(context.Request.Method))
        {
            await _next(context);
            return;
        }

        // Check if Idempotency-Key header is present
        if (!context.Request.Headers.TryGetValue(IdempotencyKeyHeader, out var idempotencyKeyValue))
        {
            await _next(context);
            return;
        }

        var idempotencyKey = idempotencyKeyValue.ToString().Trim();

        // Validate idempotency key format
        if (string.IsNullOrEmpty(idempotencyKey) || idempotencyKey.Length > 255)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid Idempotency-Key header" });
            return;
        }

        var originalBodyStream = context.Response.Body;

        try
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Check if this request has already been processed
                var storedResult = await dbContext.IdempotencyResults
                    .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey);

                if (storedResult != null)
                {
                    _logger.LogInformation(
                        "Idempotent request detected. Key: {IdempotencyKey}. Returning cached response.",
                        idempotencyKey);

                    context.Response.StatusCode = storedResult.StatusCode;
                    context.Response.Headers[IdempotencyStatusHeader] = "cached";

                    // Restore cached response
                    if (!string.IsNullOrEmpty(storedResult.ResponseBody))
                    {
                        context.Response.ContentType = storedResult.ContentType ?? "application/json";
                        await context.Response.WriteAsync(storedResult.ResponseBody);
                    }

                    return;
                }
            }

            // Capture the original response stream
            using var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            try
            {
                await _next(context);
            }
            finally
            {
                // Read the response body
                responseStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

                // Store idempotency result for successful requests
                if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                {
                    using (var scope = _serviceProvider.CreateAsyncScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                        var idempotencyResult = new IdempotencyResult
                        {
                            IdempotencyKey = idempotencyKey,
                            StatusCode = context.Response.StatusCode,
                            ResponseBody = responseBody,
                            ContentType = context.Response.ContentType,
                            CreatedAt = DateTime.UtcNow,
                            ExpiresAt = DateTime.UtcNow.AddHours(24) // Cache for 24 hours
                        };

                        dbContext.IdempotencyResults.Add(idempotencyResult);
                        await dbContext.SaveChangesAsync();

                        _logger.LogInformation(
                            "Idempotency result stored. Key: {IdempotencyKey}",
                            idempotencyKey);
                    }
                }

                // Write the response to the original stream
                await responseStream.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing idempotency key: {IdempotencyKey}", idempotencyKey);
            // Don't fail the request due to idempotency processing errors
            context.Response.Body = originalBodyStream;
            await _next(context);
        }
    }

    private static bool IsIdempotentRequest(string method) =>
        method.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
        method.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
        method.Equals("PATCH", StringComparison.OrdinalIgnoreCase) ||
        method.Equals("DELETE", StringComparison.OrdinalIgnoreCase);
} //EXPLANATION : This middleware checks for the presence of an "Idempotency-Key" header in incoming requests. If the header is present, it looks up the key in the database to see if a response has already been stored for that key. If a cached response is found, it returns that response immediately without processing the request again. If no cached response exists, it captures the response from the downstream processing and stores it in the database along with the idempotency key for future reference. This allows clients to safely retry requests without worrying about duplicate side effects, as the server will return the same result for the same idempotency key. To test this in postman you can send a POST request to an endpoint with the "Idempotency-Key" header set to a unique value. If you send the same request again with the same key, you should receive the cached response instead of processing the request again. You can also check the database to see that the idempotency result is stored correctly.