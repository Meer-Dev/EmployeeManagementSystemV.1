using EmployeeManagement.Application.Common.Interfaces;
using FluentValidation;
using System.Net;

namespace EmployeeManagement.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Resolve the scoped exception logger once per request
            var exceptionLogger = httpContext.RequestServices.GetRequiredService<IExceptionLogger>();

            try
            {
                await _next(httpContext);
            }
            catch (ValidationException ex)
            {
                _logger.LogError(ex, "Validation error occurred");

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                await exceptionLogger.LogAsync(
                    ex,
                    httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null,
                    httpContext.Request.Method,
                    httpContext.Response.StatusCode,
                    httpContext.RequestAborted);

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "Validation error",
                    Errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                });
            }

            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Key not found");

                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                await exceptionLogger.LogAsync(
                    ex,
                    httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null,
                    httpContext.Request.Method,
                    httpContext.Response.StatusCode,
                    httpContext.RequestAborted);

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    ex.Message
                });
            }

            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violated");

                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                await exceptionLogger.LogAsync(
                    ex,
                    httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null,
                    httpContext.Request.Method,
                    httpContext.Response.StatusCode,
                    httpContext.RequestAborted);

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    ex.Message
                });
            }
            

            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await exceptionLogger.LogAsync(
                    ex,
                    httpContext.Request.Path.HasValue ? httpContext.Request.Path.Value : null,
                    httpContext.Request.Method,
                    httpContext.Response.StatusCode,
                    httpContext.RequestAborted);

                await httpContext.Response.WriteAsJsonAsync(new
                {
                    Success = false,
                    Message = "An unexpected error occurred. Please try again later."
                });
            }

        }
    }
}
