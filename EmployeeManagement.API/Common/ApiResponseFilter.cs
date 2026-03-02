using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmployeeManagement.API.Common
{
    /// <summary>
    /// Global result filter that wraps successful controller responses
    /// in a standardized <see cref="ApiResponse"/> object.
    /// 
    /// This lets controllers return their normal results (e.g. Ok(employee))
    /// without manually creating ApiResponse in every action.
    /// </summary>
    public class ApiResponseFilter(ILogger<ApiResponseFilter> logger) : IAsyncResultFilter
    {
        private readonly ILogger<ApiResponseFilter> _logger = logger;

        public async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next)
        {
            var statusCode = GetStatusCode(context);
            var correlationId = context.HttpContext.Items.ContainsKey("CorrelationId")
                ? context.HttpContext.Items["CorrelationId"]?.ToString()
                : null;

            if (context.Result is ObjectResult objectResult &&
                objectResult.Value is not ApiResponse)
            {
                var isSuccess = IsSuccessStatusCode(statusCode);

                var response = new ApiResponse
                {
                    Success = isSuccess,
                    Message = GetMessageForStatusCode(statusCode),
                    Data = objectResult.Value,
                    CorrelationId = correlationId
                };

                objectResult.Value = response;
                _logger.LogInformation(
                    "API Response: Status={StatusCode}, Success={Success}, CorrelationId={CorrelationId}",
                    statusCode, isSuccess, correlationId);
            }
            else if (context.Result is BadRequestResult or NotFoundResult or UnauthorizedResult)
            {
                var errorResponse = new ApiResponse
                {
                    Success = false,
                    Message = GetMessageForStatusCode(statusCode),
                    Data = null,
                    CorrelationId = correlationId
                };

                context.Result = new ObjectResult(errorResponse) { StatusCode = statusCode };
                _logger.LogWarning(
                    "API Error: Status={StatusCode}, CorrelationId={CorrelationId}",
                    statusCode, correlationId);
            }

            await next();
        }

        private static int GetStatusCode(ResultExecutingContext context)
            => (context.Result as ObjectResult)?.StatusCode ?? context.HttpContext.Response.StatusCode;

        private static bool IsSuccessStatusCode(int statusCode)
            => statusCode >= 200 && statusCode < 300;

        private static string GetMessageForStatusCode(int statusCode) => statusCode switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            409 => "Conflict",
            500 => "Internal Server Error",
            503 => "Service Unavailable",
            _ => "Unknown Status"
        };
    }
}

/*this filter checks if the action result is an ObjectResult that does not already contain an ApiResponse. If so, it wraps the original value in a new ApiResponse object, setting the Success property based on the HTTP status code (2xx indicates success). This allows controllers to return their normal results without worrying about constructing ApiResponse manually, while still ensuring a consistent response format across the API.

ObjectResult is a base class for action results that return an object. It is commonly used for returning JSON responses from API controllers. By checking if the result is an ObjectResult and its value is not already an ApiResponse, we can ensure that we only wrap the response once and avoid double-wrapping if a controller action already returns an ApiResponse directly.
*/