
namespace EmployeeManagement.API.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public string? CorrelationId { get; set; }
}

//This is standarized way to structure API responses in a consistent format. By using a common response wrapper like ApiResponse, we can ensure that all API endpoints return data in a predictable way, which simplifies error handling and client-side processing. The Success property indicates whether the request was successful, the Message property can provide additional context or error information, and the Data property can hold any relevant data being returned from the API.