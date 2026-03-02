namespace EmployeeManagement.Domain.Entities;

/// <summary>
/// Stores the result of idempotent requests to prevent duplicate processing.
/// </summary>
public class IdempotencyResult
{
    public int Id { get; set; }
    public required string IdempotencyKey { get; set; }
    public int StatusCode { get; set; }
    public required string ResponseBody { get; set; }
    public string? ContentType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}