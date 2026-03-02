using System.Diagnostics; 

namespace EmployeeManagement.API.Middleware 
{
    // Custom middleware that logs basic information about each HTTP request and how long it took.
    // Constructor parameters:
    // - RequestDelegate next: delegate for the next middleware in the pipeline.
    // - ILogger<RequestLoggingMiddleware> logger: logger instance for writing log entries.
    public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;

        private readonly ILogger<RequestLoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                var stopwatch = Stopwatch.StartNew();
                await _next(context);
                stopwatch.Stop();

                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            else
            {
                await _next(context);
            }

        }
    }
}

