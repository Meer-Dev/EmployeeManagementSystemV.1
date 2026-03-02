namespace EmployeeManagement.Domain.Entities;

public class ExceptionLog
{
    public int Id { get; set; }

    public string ExceptionType { get; set; } = default!;

    public string Message { get; set; } = default!; // we added ! to indicate that this property is required and will not be null, even though it's a string. This is a common practice in C# to avoid nullable reference type warnings when we know that a property will always have a value.

    public string? StackTrace { get; set; }

    public string? Path { get; set; }

    public string? Method { get; set; }

    public int? StatusCode { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

