# CQRS Architecture Sample for Employee Management System

## Architecture Diagram

```
???????????????????????????????????????????????????????????????????????
?                         API Controller Layer                        ?
?                    (EmployeesController)                            ?
???????????????????????????????????????????????????????????????????????
     ?                                 ?
     ? (Commands)                      ? (Queries)
????????????????????            ????????????????????
?   MediatR Bus    ?            ?   MediatR Bus    ?
????????????????????            ????????????????????
     ?                                ?
     ?                                ?
????????????????????????????????  ????????????????????
?  Command Handlers Layer      ?  ? Query Handlers   ?
?                              ?  ?                  ?
? - CreateEmployeeHandler      ?  ? - GetEmployeeById?
? - UpdateEmployeeHandler      ?  ? - GetEmployees   ?
? - DeleteEmployeeHandler      ?  ????????????????????
????????????????????????????????           ?
     ?                                      ?
     ?                                      ?
????????????????????????????????  ????????????????????????
?   Write Database (OLTP)      ?  ?  Read Database (Opt) ?
?   - Normalized schema        ?  ?  - Denormalized views?
?   - Event Store              ?  ?  - Projections       ?
????????????????????????????????  ????????????????????????
     ?
     ?
????????????????????????????????
?   Domain Events Published    ?
?   (Event Bus - MediatR)      ?
????????????????????????????????
     ?
     ?
????????????????????????????????
?  Event Handlers/Projectors   ?
?  (Update Read Database)      ?
????????????????????????????????
```

## Project Structure

```
EmployeeManagement/
??? EmployeeManagement.API/
?   ??? Controllers/
?       ??? EmployeesController.cs
?
??? EmployeeManagement.Domain/
?   ??? Entities/
?   ?   ??? Employee.cs
?   ??? Events/
?       ??? EmployeeCreatedEvent.cs
?       ??? EmployeeUpdatedEvent.cs
?       ??? EmployeeDeletedEvent.cs
?
??? EmployeeManagement.Application/
?   ??? Employees/
?   ?   ??? Commands/
?   ?   ?   ??? CreateEmployee/
?   ?   ?   ?   ??? CreateEmployeeCommand.cs
?   ?   ?   ?   ??? CreateEmployeeCommandHandler.cs
?   ?   ?   ??? UpdateEmployee/
?   ?   ?   ??? DeleteEmployee/
?   ?   ??? Queries/
?   ?   ?   ??? GetEmployeeById/
?   ?   ?   ?   ??? GetEmployeeByIdQuery.cs
?   ?   ?   ?   ??? GetEmployeeByIdQueryHandler.cs
?   ?   ?   ??? GetEmployees/
?   ?   ??? EventHandlers/
?   ?       ??? EmployeeCreatedEventHandler.cs
?   ?       ??? EmployeeUpdatedEventHandler.cs
?   ?       ??? EmployeeDeletedEventHandler.cs
?   ?
?   ??? Common/Interfaces/
?       ??? IWriteDbContext.cs
?       ??? IReadDbContext.cs
?       ??? IEventStore.cs
?       ??? IEventBus.cs
?
??? EmployeeManagement.Infrastructure/
    ??? Persistence/
    ?   ??? WriteDbContext.cs  (OLTP - normalized)
    ?   ??? ReadDbContext.cs   (Optimized for queries)
    ?   ??? EventStore.cs      (Event sourcing)
    ??? Events/
        ??? EventBus.cs        (MediatR-based)
```

## Code Samples

### 1. Domain Layer - Domain Events

**File: Domain/Events/EmployeeCreatedEvent.cs**
```csharp
namespace EmployeeManagement.Domain.Events;

public record EmployeeCreatedEvent(
    int EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    DateTime OccurredAt = default) : IDomainEvent
{
    public DateTime OccurredAt { get; } = OccurredAt == default 
        ? DateTime.UtcNow 
        : OccurredAt;
}

public record EmployeeUpdatedEvent(
    int EmployeeId,
    string FirstName,
    string LastName,
    string Email,
    DateTime OccurredAt = default) : IDomainEvent
{
    public DateTime OccurredAt { get; } = OccurredAt == default 
        ? DateTime.UtcNow 
        : OccurredAt;
}

public record EmployeeDeletedEvent(
    int EmployeeId,
    DateTime OccurredAt = default) : IDomainEvent
{
    public DateTime OccurredAt { get; } = OccurredAt == default 
        ? DateTime.UtcNow 
        : OccurredAt;
}

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
```

### 2. Application Layer - Commands & Events

**File: Application/Employees/Commands/CreateEmployee/CreateEmployeeCommand.cs**
```csharp
using MediatR;

namespace EmployeeManagement.Application.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    string Email) : IRequest<int>;
```

**File: Application/Employees/Commands/CreateEmployee/CreateEmployeeCommandHandler.cs**
```csharp
using MediatR;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Events;
using EmployeeManagement.Application.Common.Interfaces;

namespace EmployeeManagement.Application.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, int>
{
    private readonly IWriteAppDbContext _writeDb;
    private readonly IEventBus _eventBus;

    public CreateEmployeeCommandHandler(
        IWriteAppDbContext writeDb,
        IEventBus eventBus)
    {
        _writeDb = writeDb;
        _eventBus = eventBus;
    }

    public async Task<int> Handle(
        CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        // Create employee in write database
        var employee = new Employee
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email
        };

        _writeDb.AddEmployee(employee);
        await _writeDb.SaveChangesAsync(cancellationToken);

        // Publish domain event to event bus
        var @event = new EmployeeCreatedEvent(
            employee.Id,
            employee.FirstName,
            employee.LastName,
            employee.Email);

        await _eventBus.PublishAsync(@event, cancellationToken);

        return employee.Id;
    }
}
```

### 3. Application Layer - Event Handlers (Projections)

**File: Application/Employees/EventHandlers/EmployeeCreatedEventHandler.cs**
```csharp
using MediatR;
using EmployeeManagement.Domain.Events;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Infrastructure.Persistence.ReadModels;

namespace EmployeeManagement.Application.Employees.EventHandlers;

public class EmployeeCreatedEventHandler : INotificationHandler<EmployeeCreatedEvent>
{
    private readonly IReadAppDbContext _readDb;

    public EmployeeCreatedEventHandler(IReadAppDbContext readDb)
    {
        _readDb = readDb;
    }

    public async Task Handle(
        EmployeeCreatedEvent @event,
        CancellationToken cancellationToken)
    {
        // Update read database with new employee data
        var employeeReadModel = new EmployeeReadModel
        {
            Id = @event.EmployeeId,
            FirstName = @event.FirstName,
            LastName = @event.LastName,
            Email = @event.Email,
            CreatedAt = @event.OccurredAt
        };

        await _readDb.AddEmployeeReadModelAsync(employeeReadModel, cancellationToken);
    }
}

public class EmployeeUpdatedEventHandler : INotificationHandler<EmployeeUpdatedEvent>
{
    private readonly IReadAppDbContext _readDb;

    public EmployeeUpdatedEventHandler(IReadAppDbContext readDb)
    {
        _readDb = readDb;
    }

    public async Task Handle(
        EmployeeUpdatedEvent @event,
        CancellationToken cancellationToken)
    {
        await _readDb.UpdateEmployeeReadModelAsync(
            @event.EmployeeId,
            @event.FirstName,
            @event.LastName,
            @event.Email,
            cancellationToken);
    }
}

public class EmployeeDeletedEventHandler : INotificationHandler<EmployeeDeletedEvent>
{
    private readonly IReadAppDbContext _readDb;

    public EmployeeDeletedEventHandler(IReadAppDbContext readDb)
    {
        _readDb = readDb;
    }

    public async Task Handle(
        EmployeeDeletedEvent @event,
        CancellationToken cancellationToken)
    {
        await _readDb.DeleteEmployeeReadModelAsync(@event.EmployeeId, cancellationToken);
    }
}
```

### 4. Application Layer - Queries

**File: Application/Employees/Queries/GetEmployees/GetEmployeesQuery.cs**
```csharp
using MediatR;
using EmployeeManagement.Application.Employees.Dtos;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees;

public record GetEmployeesQuery : IRequest<List<EmployeeDto>>;
```

**File: Application/Employees/Queries/GetEmployees/GetEmployeesQueryHandler.cs**
```csharp
using MediatR;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Common.Interfaces;

namespace EmployeeManagement.Application.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, List<EmployeeDto>>
{
    private readonly IReadAppDbContext _readDb;

    public GetEmployeesQueryHandler(IReadAppDbContext readDb)
    {
        _readDb = readDb;
    }

    public async Task<List<EmployeeDto>> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        // Query from optimized read database
        return await _readDb.GetAllEmployeesAsync(cancellationToken);
    }
}
```

### 5. Infrastructure Layer - Interfaces

**File: Application/Common/Interfaces/IEventBus.cs**
```csharp
using EmployeeManagement.Domain.Events;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IEventBus
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent;
}
```

**File: Application/Common/Interfaces/IWriteAppDbContext.cs**
```csharp
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IWriteAppDbContext
{
    void AddEmployee(Employee employee);
    void UpdateEmployee(Employee employee);
    void DeleteEmployee(Employee employee);
    Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

**File: Application/Common/Interfaces/IReadAppDbContext.cs**
```csharp
using EmployeeManagement.Application.Employees.Dtos;

namespace EmployeeManagement.Application.Common.Interfaces;

public interface IReadAppDbContext
{
    Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken);
    Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken);
    Task AddEmployeeReadModelAsync(
        EmployeeReadModel model,
        CancellationToken cancellationToken);
    Task UpdateEmployeeReadModelAsync(
        int id,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken);
    Task DeleteEmployeeReadModelAsync(int id, CancellationToken cancellationToken);
}

public class EmployeeReadModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 6. Infrastructure Layer - Event Bus Implementation

**File: Infrastructure/Events/EventBus.cs**
```csharp
using MediatR;
using EmployeeManagement.Domain.Events;
using EmployeeManagement.Application.Common.Interfaces;

namespace EmployeeManagement.Infrastructure.Events;

public class EventBus : IEventBus
{
    private readonly IPublisher _publisher;

    public EventBus(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : IDomainEvent
    {
        // Publish event as MediatR notification
        await _publisher.Publish(@event, cancellationToken);
    }
}
```

### 7. Infrastructure Layer - Write Database (OLTP)

**File: Infrastructure/Persistence/WriteDbContext.cs**
```csharp
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence;

public class WriteDbContext : DbContext, IWriteAppDbContext
{
    public DbSet<Employee> Employees => Set<Employee>();

    public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options) { }

    public void AddEmployee(Employee employee) => Employees.Add(employee);

    public void UpdateEmployee(Employee employee) => Employees.Update(employee);

    public void DeleteEmployee(Employee employee) => Employees.Remove(employee);

    public async Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        => await Employees.FindAsync(new object[] { id }, cancellationToken: cancellationToken);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Normalized schema for write database
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
```

### 8. Infrastructure Layer - Read Database (Optimized)

**File: Infrastructure/Persistence/ReadDbContext.cs**
```csharp
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Infrastructure.Persistence;

public class ReadDbContext : DbContext, IReadAppDbContext
{
    public DbSet<EmployeeReadModel> EmployeeReadModels => Set<EmployeeReadModel>();

    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken)
        => await EmployeeReadModels
            .AsNoTracking()
            .Where(e => e.Id == id)
            .Select(e => new EmployeeDto(e.Id, e.FirstName, e.LastName, e.Email))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<List<EmployeeDto>> GetAllEmployeesAsync(CancellationToken cancellationToken)
        => await EmployeeReadModels
            .AsNoTracking()
            .Select(e => new EmployeeDto(e.Id, e.FirstName, e.LastName, e.Email))
            .ToListAsync(cancellationToken);

    public async Task AddEmployeeReadModelAsync(
        EmployeeReadModel model,
        CancellationToken cancellationToken)
    {
        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = DateTime.UtcNow;
        EmployeeReadModels.Add(model);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateEmployeeReadModelAsync(
        int id,
        string firstName,
        string lastName,
        string email,
        CancellationToken cancellationToken)
    {
        var model = await EmployeeReadModels.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (model != null)
        {
            model.FirstName = firstName;
            model.LastName = lastName;
            model.Email = email;
            model.UpdatedAt = DateTime.UtcNow;
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteEmployeeReadModelAsync(int id, CancellationToken cancellationToken)
    {
        var model = await EmployeeReadModels.FindAsync(new object[] { id }, cancellationToken: cancellationToken);
        if (model != null)
        {
            EmployeeReadModels.Remove(model);
            await SaveChangesAsync(cancellationToken);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Denormalized/optimized schema for read database
        modelBuilder.Entity<EmployeeReadModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
            // Add indexes for better read performance
            entity.HasIndex(e => e.FirstName);
            entity.HasIndex(e => e.LastName);
        });

        base.OnModelCreating(modelBuilder);
    }
}
```

### 9. API Controller

**File: API/Controllers/EmployeesController.cs**
```csharp
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Application.Employees.Commands.CreateEmployee;
using EmployeeManagement.Application.Employees.Queries.GetEmployees;
using EmployeeManagement.Application.Employees.Queries.GetEmployeeById;

namespace EmployeeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<EmployeeDto>>> GetEmployees(CancellationToken cancellationToken)
    {
        var query = new GetEmployeesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDto>> GetEmployeeById(int id, CancellationToken cancellationToken)
    {
        var query = new GetEmployeeByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<int>> CreateEmployee(
        CreateEmployeeCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetEmployeeById), new { id = result }, result);
    }
}
```

### 10. Dependency Injection Setup

**File: Infrastructure/DependencyInjection.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Infrastructure.Events;
using EmployeeManagement.Infrastructure.Persistence;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string writeConnectionString,
        string readConnectionString)
    {
        // Register Write Database (OLTP)
        services.AddDbContext<WriteDbContext>(options =>
            options.UseSqlServer(writeConnectionString));

        services.AddScoped<IWriteAppDbContext>(
            provider => provider.GetRequiredService<WriteDbContext>());

        // Register Read Database (Optimized)
        services.AddDbContext<ReadDbContext>(options =>
            options.UseSqlServer(readConnectionString));

        services.AddScoped<IReadAppDbContext>(
            provider => provider.GetRequiredService<ReadDbContext>());

        // Register Event Bus
        services.AddScoped<IEventBus, EventBus>();

        // Register MediatR (automatically discovers handlers)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(EventBus).Assembly));

        return services;
    }
}
```

## Key Differences: Your Current vs CQRS Architecture

| Aspect | Current | CQRS |
|--------|---------|------|
| **Databases** | Single | Two (Write + Read) |
| **Write Path** | Direct DB insert | Write DB ? Event ? Read DB |
| **Read Path** | Same normalized DB | Optimized read DB |
| **Event Management** | None | Full event sourcing |
| **Scalability** | Limited | Highly scalable |
| **Consistency** | Strong | Eventual consistency |
| **Complexity** | Low | Higher |

## Benefits of Full CQRS

? **Separate scaling** - Scale read/write independently  
? **Event sourcing** - Complete audit trail  
? **Optimized queries** - Denormalized read models  
? **Better performance** - No complex joins on reads  
? **Event-driven** - Publish/subscribe pattern  
? **Microservices ready** - Easy to split later  

Would you like me to help implement this full CQRS architecture in your project?
