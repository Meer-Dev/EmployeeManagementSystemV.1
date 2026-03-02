namespace EmployeeManagement.API.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ILogger<CorrelationIdMiddleware> logger)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append(CorrelationIdHeader, correlationId);

        using (logger.BeginScope(new Dictionary<string, object> { { "CorrelationId", correlationId } }))
        {
            logger.LogInformation("Request started with Correlation ID: {CorrelationId}", correlationId);
            await _next(context);
            logger.LogInformation("Request completed with Correlation ID: {CorrelationId}", correlationId);
        }
    }
}