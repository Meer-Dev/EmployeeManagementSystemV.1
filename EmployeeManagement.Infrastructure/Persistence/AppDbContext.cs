using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EmployeeManagement.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<ExceptionLog> ExceptionLogs => Set<ExceptionLog>();
    public DbSet<Employee> Employees { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<IdempotencyResult> IdempotencyResults { get; set; } = null!;

    // ----------------------------
    // Write Operations
    // ----------------------------

    public void AddEmployee(Employee employee)
        => Employees.Add(employee);

    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        => await Employees.FindAsync([id], cancellationToken);

    public void DeleteEmployee(Employee employee)
        => Employees.Remove(employee);

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    // ----------------------------
    // Configuration of EF Core Behavior
    // ----------------------------

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        
        // Suppress pending model changes warning during startup
        optionsBuilder.ConfigureWarnings(w => 
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    } 
    // what it does in real world is suppressing warnings about pending model changes during startup because we might be applying migrations at runtime or have a separate migration assembly. This keeps the logs cleaner and avoids confusion about model state during normal operations. for example if we have a separate migration assembly or apply migrations at runtime, EF Core might warn about pending model changes when the application starts. This is expected and not an issue, so we can safely ignore that specific warning to keep our logs cleaner and avoid confusion about the model state during normal operations.

    // ----------------------------
    // Index Configuration
    // ----------------------------

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(builder =>
        {
            // Primary key (auto indexed)
            builder.HasKey(e => e.Id);

            // Single-column indexes for filtering
            builder.HasIndex(e => e.FirstName);
            builder.HasIndex(e => e.LastName);
            builder.HasIndex(e => e.Department);
            builder.HasIndex(e => e.IsActive);

            // Email should usually be unique
            builder.HasIndex(e => e.Email)
                   .IsUnique();

            // Composite index for keyset pagination (important)
            builder.HasIndex(e => new { e.Id });

            // If you frequently filter by LastName and sort by Id
            builder.HasIndex(e => new { e.LastName, e.Id });

            // Configure RefreshTokens relationship
            builder.HasMany(e => e.RefreshTokens)
                .WithOne(rt => rt.Employee)
                .HasForeignKey(rt => rt.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });

        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.HasKey(rt => rt.Id);
            builder.HasIndex(rt => new { rt.EmployeeId, rt.Token });
            builder.HasIndex(rt => rt.ExpiresAt);
        });

        // Configure IdempotencyResult
        modelBuilder.Entity<IdempotencyResult>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<IdempotencyResult>()
            .Property(x => x.IdempotencyKey)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<IdempotencyResult>()
            .HasIndex(x => x.IdempotencyKey)
            .IsUnique();

        modelBuilder.Entity<IdempotencyResult>()
            .HasIndex(x => x.ExpiresAt);


    }
}
