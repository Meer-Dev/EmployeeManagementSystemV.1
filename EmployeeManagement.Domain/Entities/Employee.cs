namespace EmployeeManagement.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; set; } = "User";

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    // Navigation property for refresh tokens
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    private Employee() { }

    public Employee(string firstName, string lastName, string email, string department, string passwordHash, string role = "User")
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Department = department;
        PasswordHash = passwordHash;
        Role = role;
        IsActive = true;
    }

    public Employee(string firstName, string lastName, string email, string department, bool isActive)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Department = department;
        IsActive = isActive;
    }

    public void Update(string firstName, string lastName, string email, string department, bool isActive)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Department = department;
        IsActive = isActive;
    }

    public void UpdateFirstName(string firstName) => FirstName = firstName;
    public void UpdateLastName(string lastName) => LastName = lastName;
    public void UpdateEmail(string email) => Email = email;
    public void UpdateDepartment(string department) => Department = department;

    public void UpdateIsActive(bool isActive) => IsActive = isActive;

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("Employee is already inactive.");
        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("Employee is already active.");
        IsActive = true;
    }

    public void SetPassword(string hashedPassword) => PasswordHash = hashedPassword;
    public void SetRole(string role) => Role = role;
}
