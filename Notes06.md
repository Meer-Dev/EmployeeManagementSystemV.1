# 📚 Week 06 Master Notes: Testing, Background Jobs & Deployment
## EmployeeManagement API - Complete Expert Guide

> *"The expert has forgotten more than you know, but remembers why it matters."*

---

## 🎯 Week 6 Objective Recap
```
✅ Unit testing (xUnit)
✅ Mocking (Moq) 
✅ Testing MediatR handlers
✅ Background jobs (Hangfire)
✅ Domain events
✅ Redis caching
✅ GraphQL (basic overview)
✅ API documentation
✅ Deployment
```

---

# 🧪 PART 1: UNIT TESTING WITH xUNIT

## 🔹 What You Built
```csharp
public class UpdateEmployeeCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _contextMock;
    private readonly UpdateEmployeeCommandHandler _handler;

    public UpdateEmployeeCommandHandlerTests()
    {
        _contextMock = new Mock<IUnitOfWork>();
        _handler = new UpdateEmployeeCommandHandler(_contextMock.Object);
    }
    // ... test methods
}
```

## 🔹 WHY xUnit? (The Decision)

| Framework | Why xUnit Won | When to Choose Others |
|-----------|--------------|---------------------|
| **xUnit** | ✅ Modern, extensible, parallel by default, [Fact]/[Theory] attributes | - |
| NUnit | - | If team already uses NUnit or needs more built-in assertions |
| MSTest | - | If working strictly in Microsoft ecosystem with legacy code |

### Behind The Scenes (BTS): How xUnit Works
```
1. Test Discovery: xUnit scans assemblies for [Fact] or [Theory] methods
2. Test Execution: Creates new instance of test class per test (isolation!)
3. Assertion: FluentAssertions wraps actual/expected comparison
4. Reporting: Results sent to test runner (Visual Studio Test Explorer, CLI, CI/CD)
```

### Real-World Analogy 🏭
> Think of xUnit as a **quality control inspector** in a factory:
> - Each `[Fact]` = one product check
> - `Mock<IUnitOfWork>` = simulated assembly line (no real database needed)
> - `Should().Be()` = pass/fail gauge
> - Parallel execution = multiple inspectors working simultaneously

---

## 🔹 MOCKING WITH Moq: The Art of Isolation

### What You Did
```csharp
_contextMock.Setup(x => x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
    .ReturnsAsync(employee);
```

### WHY Mock? (The Philosophy)
```
❌ WITHOUT Mocking:
Test → Real Database → Slow, Flaky, Needs Setup, Can't Test Edge Cases

✅ WITH Mocking:
Test → Controlled Fake → Fast, Reliable, Test Any Scenario
```

### Moq Setup Patterns You Used

| Pattern | Code Example | When to Use |
|---------|-------------|-------------|
| **ReturnsAsync** | `.ReturnsAsync(employee)` | Async methods returning data |
| **ReturnsAsync(null)** | `.ReturnsAsync((Employee?)null)` | Testing "not found" scenarios |
| **It.IsAny<T>()** | `It.IsAny<CancellationToken>()` | When parameter value doesn't matter |
| **Verify** | `.Verify(x => x.SaveChangesAsync(...), Times.Once)` | Asserting side effects happened |

### BTS: How Moq Works Internally
```csharp
// Simplified Moq magic:
1. Mock<T>() creates a dynamic proxy using Castle.DynamicProxy
2. .Setup() records your expectation in a dictionary
3. When method is called, proxy checks dictionary for matching setup
4. Returns your configured value instead of real implementation
5. .Verify() checks if recorded calls match expectations
```

### ⚠️ Common Pitfalls & Better Ways

```csharp
// ❌ BAD: Over-mocking (testing mocks, not logic)
_mockRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(employee);
_mockRepo.Setup(x => x.UpdateAsync(employee)).ReturnsAsync(employee);
// Now you're testing Moq, not your handler!

// ✅ GOOD: Mock only dependencies, test behavior
_mockRepo.Setup(x => x.GetByIdAsync(1, ...)).ReturnsAsync(employee);
// Let handler call SaveChangesAsync, then Verify it was called
```

### Alternatives to Moq
| Library | Pros | Cons | Best For |
|---------|------|------|----------|
| **Moq** | Easy syntax, popular, good docs | Can encourage over-mocking | Most projects |
| **NSubstitute** | More readable, fewer setup steps | Slightly less flexible | Teams preferring readability |
| **FakeItEasy** | Very intuitive, strict by default | Smaller community | Strict behavior testing |
| **Manual Fakes** | Full control, no library dependency | More code to maintain | Simple scenarios, learning |

---

## 🔹 TESTING MEDIATR HANDLERS: CQRS Testing Strategy

### Your Handler Pattern
```csharp
public class UpdateEmployeeCommandHandler(IUnitOfWork context) 
    : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await context.GetEmployeeByIdAsync(request.Id, ct);
        if (employee == null) throw new InvalidOperationException(...);
        
        employee.Update(request.FirstName, request.LastName, ...);
        await context.SaveChangesAsync(ct);
    }
}
```

### WHY Test Handlers Directly?
```
✅ Benefits:
- Fast (no HTTP overhead)
- Isolated (test business logic only)
- Easy to mock dependencies
- Catches logic bugs before integration

⚠️ Limitations:
- Doesn't test routing/model binding
- Doesn't test middleware pipeline
- Need separate integration tests for full flow
```

### Testing Strategy Pyramid for Your API
```
        /\
       /E2E\      ← 5% - Full HTTP tests (TestServer)
      /------\
     /Integration\ ← 15% - Handler + DB (Testcontainers)
    /--------------\
   /   Unit Tests   \ ← 80% - Handler logic with mocks (You are here!)
  /------------------\
```

### Better Way: Add Integration Tests Later
```csharp
// Future: Test with real database using Testcontainers
[Fact]
public async Task UpdateEmployee_IntegrationTest()
{
    await using var container = new PostgreSqlBuilder().Build();
    await container.StartAsync();
    
    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(container.GetConnectionString())
        .Options;
    
    await using var context = new AppDbContext(options);
    await context.Database.MigrateAsync();
    
    // Now test with REAL database
    var handler = new UpdateEmployeeCommandHandler(context);
    // ... rest of test
}
```

---

# ⚙️ PART 2: BACKGROUND JOBS WITH HANGFIRE

## 🔹 What You Configured
```csharp
// In Program.cs
builder.Services.AddHangfire(config => config.UseMemoryStorage());
builder.Services.AddHangfireServer();

// Recurring job
RecurringJob.AddOrUpdate<IEmployeeReportService>(
    "DailyEmployeeReport",
    service => service.GenerateDailyReport(),
    Cron.Daily
);

// Dashboard
app.UseHangfireDashboard("/hangfire");
```

## 🔹 WHY Hangfire? (The Decision Matrix)

| Requirement | Hangfire | Quartz.NET | BackgroundService | Azure Functions |
|-------------|----------|------------|------------------|----------------|
| **Simple recurring jobs** | ✅ Easy | ⚠️ Complex | ✅ Good | ⚠️ Overkill |
| **Persistent queue** | ✅ Built-in | ✅ Built-in | ❌ Manual | ✅ Built-in |
| **Dashboard/UI** | ✅ Beautiful | ⚠️ Third-party | ❌ None | ✅ Azure Portal |
| **Retry logic** | ✅ Automatic | ⚠️ Manual | ❌ Manual | ✅ Built-in |
| **Learning curve** | 🟢 Low | 🔴 High | 🟢 Low | 🟡 Medium |

### BTS: How Hangfire Works Internally
```
1. Job Registration: AddOrUpdate() stores job definition in storage
2. Server Polling: Hangfire Server checks storage every ~15 seconds
3. Job Execution: 
   - Fetches due job
   - Creates scope with dependencies (IEmployeeReportService)
   - Executes method in background thread
   - Updates job state (Processing → Succeeded/Failed)
4. Retry Logic: If exception thrown, re-queues with exponential backoff
5. Dashboard: Reads job state from storage, displays in real-time
```

### ⚠️ Critical: MemoryStorage vs Production

```csharp
// ❌ DEVELOPMENT ONLY (You used this)
config.UseMemoryStorage(); 
// Problem: Jobs lost on app restart, single-server only

// ✅ PRODUCTION OPTIONS:
// Option 1: SQL Server (most common)
config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));

// Option 2: Redis (high performance)
config.UseRedisStorage("localhost:6379");

// Option 3: PostgreSQL
config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
```

### Real-World Example: Daily Report Job
```csharp
public class EmployeeReportService : IEmployeeReportService
{
    public async Task GenerateDailyReport()
    {
        var activeEmployees = await _context.Employees
            .Where(e => e.IsActive)
            .ToListAsync();
        
        // BTS: This runs in background thread, NOT request thread
        // ✅ Pros: Doesn't block user requests
        // ⚠️ Cons: No HTTP context, must handle errors internally
        
        Console.WriteLine($"Daily report: {activeEmployees.Count} active employees.");
        
        // Better: Send email, upload to S3, log to monitoring system
        // await _emailService.SendReportAsync(activeEmployees);
    }
}
```

### Better Ways to Enhance Your Hangfire Setup

```csharp
// 1. Add filters for logging/authorization
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(...)
          .WithJobExpirationTimeout(TimeSpan.FromDays(7))
          .WithQueue("default");
});

// 2. Add authorization to dashboard (CRITICAL!)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() } // Only admins
});

// 3. Add retry policies
[AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
public async Task GenerateDailyReport() { ... }

// 4. Use continuations for job chains
var jobId = BackgroundJob.Enqueue<IService>(x => x.StepOne());
BackgroundJob.ContinueWith<IService>(jobId, x => x.StepTwo());
```

---

# 🗄️ PART 3: REDIS CACHING

## 🔹 What You Implemented
```csharp
// Registration
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost"));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Service Implementation
public class RedisCacheService : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return JsonSerializer.Deserialize<T>(value);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }
}
```

## 🔹 WHY Redis? (Caching Strategy)

### Cache-Aside Pattern (What You Used)
```
Application Flow:
1. Request comes in for employee data
2. Check Redis: key = "employee:123"
3. If found (CACHE HIT) → Return immediately ⚡
4. If not found (CACHE MISS) → 
   a. Query database
   b. Store result in Redis with TTL
   c. Return to client
```

### BTS: How Redis Works Internally
```
1. Connection: ConnectionMultiplexer maintains pooled connections
2. Command: StringGetAsync sends RESP protocol command to Redis server
3. Memory Lookup: Redis checks in-memory hash table (O(1) complexity!)
4. Serialization: Your JsonSerializer converts object ↔ JSON string
5. Expiry: Redis background thread removes expired keys automatically

Performance Comparison:
- Database Query: 10-100ms (disk I/O, network, query parsing)
- Redis Cache: 0.1-1ms (in-memory, optimized protocol)
→ 10-100x faster for repeated reads!
```

### ⚠️ Critical Considerations You Should Know

```csharp
// 1. Cache Invalidation Strategy (Hardest Problem in CS!)
// Your current approach: Time-based expiry (TTL)
await SetAsync("employee:123", employee, TimeSpan.FromMinutes(10));

// Better: Combine TTL + explicit invalidation
public async Task UpdateEmployeeAsync(Employee employee)
{
    // Update database
    await _unitOfWork.Employees.UpdateAsync(employee);
    
    // Invalidate cache (not update - avoid race conditions)
    await _cacheService.RemoveAsync($"employee:{employee.Id}");
    
    // Optional: Update related caches (e.g., employee list)
    await _cacheService.RemoveAsync("employees:all");
}

// 2. Cache Stampede Prevention
// Problem: Cache expires, 100 requests hit DB simultaneously
// Solution: Use distributed lock or probabilistic early expiry
public async Task<T?> GetWithStampedeProtection<T>(string key, 
    Func<Task<T>> factory, 
    TimeSpan expiry)
{
    var value = await GetAsync<T>(key);
    if (value != null) return value;
    
    // Only one request rebuilds cache
    var lockKey = $"{key}:lock";
    if (await _redis.LockTakeAsync(lockKey, "1", TimeSpan.FromSeconds(5)))
    {
        try 
        {
            value = await factory();
            await SetAsync(key, value, expiry);
        }
        finally 
        {
            await _redis.LockReleaseAsync(lockKey, "1");
        }
    }
    else
    {
        // Wait and retry (another request is rebuilding)
        await Task.Delay(100);
        return await GetAsync<T>(key);
    }
    return value;
}

// 3. Serialization Gotchas
// Your current: JsonSerializer (good, but watch for reference loops)
// Better: Add options for production
var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    ReferenceHandler = ReferenceHandler.IgnoreCycles // Prevent loops
};
```

### When NOT to Cache
```csharp
// ❌ Don't cache:
- Frequently changing data (stock prices, real-time metrics)
- User-specific data without proper key design
- Large datasets that won't fit in memory
- Data requiring strong consistency (financial transactions)

// ✅ Do cache:
- Reference data (departments, roles, configurations)
- Read-heavy, write-rare data (employee profiles)
- Expensive computations (reports, aggregations)
- API responses with rate limiting
```

### Alternatives to Redis
| Solution | Best For | Trade-offs |
|----------|----------|------------|
| **Redis** | General-purpose, pub/sub, data structures | Requires separate server, learning curve |
| **MemoryCache** | Simple, single-server apps | Lost on restart, no distributed sharing |
| **Distributed MemoryCache** | Multi-server, simple needs | Still loses on restart, no persistence |
| **SQL Server Cache** | Already using SQL Server | Slower than Redis, ties cache to DB |

---

# 🎯 PART 4: GRAPHQL WITH HOTCHOCOLATE

## 🔹 What You Set Up
```csharp
// Registration
builder.Services
    .AddGraphQLServer()
    .AddQueryType<EmployeeQuery>()
    .AddFiltering()
    .AddSorting();

// Query Type
public class EmployeeQuery
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public async Task<List<EmployeeDto>> GetEmployees()
    {
        var paged = await _repo.GetPagedAsync(pageSize: 100);
        return (List<EmployeeDto>)paged.Items;
    }
}
```

## 🔹 WHY GraphQL? (vs REST)

### REST vs GraphQL Comparison
```
REST Endpoint Example:
GET /api/employees?id=1&include=department,manager
GET /api/employees/1
GET /api/employees/1/department
→ Multiple round trips, over/under-fetching

GraphQL Single Request:
query {
  employee(id: 1) {
    firstName
    lastName
    department { name }
    manager { firstName }
  }
}
→ One request, exact fields needed
```

### BTS: How HotChocolate Processes a Query
```
1. Parsing: GraphQL query string → AST (Abstract Syntax Tree)
2. Validation: Check query against schema (types, fields, permissions)
3. Execution: 
   - Resolve each field by calling your C# methods
   - DataLoader batches N+1 queries automatically
   - Parallel execution for independent fields
4. Serialization: Result → JSON response
5. Response: Send to client with errors if any
```

### ⚠️ Important: Your Current Implementation Gap
```csharp
// Your current GetEmployees() ignores filtering/sorting parameters!
[UsePaging]
[UseFiltering] 
[UseSorting]
public async Task<List<EmployeeDto>> GetEmployees()
{
    // ❌ This doesn't use the filter/sort from GraphQL query
    var paged = await _repo.GetPagedAsync(pageSize: 100);
    return (List<EmployeeDto>)paged.Items;
}

// ✅ Better: Use HotChocolate's conventions
public async Task<IQueryable<EmployeeDto>> GetEmployees(
    [ScopedService] IEmployeeReadRepository repo,
    CancellationToken ct)
{
    // Return IQueryable so HotChocolate can apply filters/sorts
    return repo.GetQueryable(); // You'd need to add this method
}

// Or: Use HotChocolate's built-in filtering with EF Core
builder.Services
    .AddGraphQLServer()
    .AddQueryType<EmployeeQuery>()
    .AddFiltering() // Auto-generates filter input types
    .AddSorting()   // Auto-generates sort input types
    .UseDefaultFiltering() // Apply filters to IQueryable
    .UseDefaultSorting();  // Apply sorting to IQueryable
```

### Real-World GraphQL Query Examples
```graphql
# Get first 10 active employees, sorted by name
query {
  employees(where: { isActive: { eq: true } }, order: { firstName: ASC }, first: 10) {
    edges {
      node {
        id
        firstName
        lastName
        email
      }
    }
    pageInfo {
      hasNextPage
      endCursor
    }
  }
}

# Get employee with nested department
query {
  employee(id: 123) {
    firstName
    department {
      name
      budget
    }
  }
}

# Mutation: Create employee
mutation {
  createEmployee(input: {
    firstName: "Jane"
    lastName: "Doe" 
    email: "jane@company.com"
    department: "Engineering"
  }) {
    employee {
      id
      firstName
    }
  }
}
```

### When to Use GraphQL vs REST
```
✅ Use GraphQL when:
- Multiple client types (web, mobile, third-party) with different data needs
- Complex nested data relationships
- Need to reduce over-fetching on mobile networks
- Rapid frontend iteration (no backend changes for new fields)

✅ Stick with REST when:
- Simple CRUD operations
- Caching at HTTP level is critical (GraphQL uses POST by default)
- File uploads/downloads
- Team expertise is primarily REST
- Need OpenAPI/Swagger documentation auto-generation

💡 Pro Tip: You can use BOTH! 
- REST for auth, file uploads, simple operations
- GraphQL for complex data queries
```

---

# 🧩 PART 5: DOMAIN EVENTS (Conceptual - You Have the Foundation)

## 🔹 What Domain Events Are
```csharp
// Conceptual example (not in your code yet, but easy to add)
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public class EmployeeCreatedEvent : IDomainEvent
{
    public int EmployeeId { get; }
    public string Email { get; }
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    
    public EmployeeCreatedEvent(int employeeId, string email)
    {
        EmployeeId = employeeId;
        Email = email;
    }
}
```

## 🔹 WHY Domain Events? (The Power of Decoupling)

### Without Domain Events (Tightly Coupled)
```csharp
// In CreateEmployeeCommandHandler
public async Task Handle(CreateEmployeeCommand request, CancellationToken ct)
{
    var employee = new Employee(...);
    await _userManager.CreateAsync(employee, request.Password);
    
    // ❌ Tightly coupled side effects:
    await _emailService.SendWelcomeEmail(employee.Email); // What if email service is down?
    await _auditService.LogCreation(employee.Id); // What if we want to remove auditing later?
    await _analytics.TrackSignup(employee.Department); // Hard to test!
    
    return employee.Id;
}
```

### With Domain Events (Loosely Coupled)
```csharp
// In Employee entity (rich domain model)
public class Employee
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public static Employee Create(string firstName, string email, ...)
    {
        var employee = new Employee(firstName, ..., email, ...);
        employee._domainEvents.Add(new EmployeeCreatedEvent(employee.Id, employee.Email));
        return employee;
    }
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// In handler (clean responsibility)
public async Task Handle(CreateEmployeeCommand request, CancellationToken ct)
{
    var employee = Employee.Create(request.FirstName, request.Email, ...);
    await _userManager.CreateAsync(employee, request.Password);
    
    // Publish events for others to handle
    foreach (var domainEvent in employee.DomainEvents)
    {
        await _eventPublisher.PublishAsync(domainEvent, ct);
    }
    employee.ClearDomainEvents();
    
    return employee.Id;
}

// Separate handlers for each side effect (testable, replaceable)
public class SendWelcomeEmailHandler : INotificationHandler<EmployeeCreatedEvent>
{
    public async Task Handle(EmployeeCreatedEvent @event, CancellationToken ct)
    {
        await _emailService.SendWelcomeEmail(@event.Email);
    }
}

public class LogEmployeeCreationHandler : INotificationHandler<EmployeeCreatedEvent>
{
    public async Task Handle(EmployeeCreatedEvent @event, CancellationToken ct)
    {
        await _auditService.LogCreation(@event.EmployeeId);
    }
}
```

### BTS: How Domain Events Flow
```
1. Business Logic: Entity method adds event to internal list
2. Handler: After persistence, publishes events to mediator/event bus
3. Dispatch: MediatR/EventBus finds all handlers for that event type
4. Execution: Handlers run (can be in-process, or out-of-process via message queue)
5. Error Handling: If one handler fails, others can still run (configurable)
```

### When to Use Domain Events
```
✅ Perfect for:
- Sending notifications (email, SMS, push)
- Updating read models (CQRS)
- Integration with external systems (webhooks, APIs)
- Audit logging and analytics
- Cache invalidation

❌ Avoid for:
- Core business logic that must succeed together (use transactions instead)
- Simple CRUD with no side effects (over-engineering)
- When you need immediate consistency across aggregates
```

---

# 🚀 PART 6: DEPLOYMENT & PRODUCTION READINESS

## 🔹 Your Current Configuration Highlights
```json
// appsettings.json production considerations
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-db;Database=EmployeeDb;..."
  },
  "Jwt": {
    "Key": "USE_A_STRONG_SECRET_FROM_ENVIRONMENT!", // ❌ Don't hardcode
    "ExpiryMinutes": "60",
    "Issuer": "EmployeeManagementAPI",
    "Audience": "EmployeeManagementClient"
  },
  "RateLimiting": {
    "FixedWindow": { "Window": "00:01:00", "PermitLimit": 100 },
    "SlidingWindow": { "Window": "00:01:00", "PermitLimit": 2 } // Auth endpoints
  }
}
```

## 🔹 Deployment Checklist: From Code to Production

### 1. Configuration Management (CRITICAL)
```bash
# ❌ BAD: Secrets in code/appsettings.json
"Jwt": { "Key": "MySuperSecretKey123" }

# ✅ GOOD: Use environment variables or secret manager
# In Program.cs
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddAzureKeyVault(...); // Or AWS Secrets Manager

# Deploy with:
export Jwt__Key="prod-secret-from-vault"
export ConnectionStrings__DefaultConnection="prod-connection-string"
```

### 2. Database Migrations Strategy
```csharp
// Your current approach (in Program.cs startup):
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync(); // Applies migrations on startup
}

// ✅ This works for small apps, but consider:

// Option A: Apply migrations during CI/CD pipeline (better for zero-downtime)
// dotnet ef database update --connection "prod-connection"

// Option B: Use migration assembly for separate deployment
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, 
        sql => sql.MigrationsAssembly("EmployeeManagement.Migrations")));

// Option C: Health check before accepting traffic
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = check => check.Name == "database"
});
```

### 3. Logging & Monitoring (You Have Serilog - Great!)
```csharp
// Your current Serilog setup:
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// ✅ Production Enhancements:
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // External config
    .Enrich.WithProperty("Application", "EmployeeAPI")
    .Enrich.WithProperty("Environment", env) // Dev/Staging/Prod
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Hour, retainedFileCountLimit: 24)
    .WriteTo.Seq("http://seq-server:5341") // Centralized logging
    .CreateLogger();

// Add correlation ID to all logs (you have middleware for this - perfect!)
// Now every log entry includes X-Correlation-ID for tracing requests
```

### 4. Health Checks (Missing - Add This!)
```csharp
// In Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database")
    .AddRedis("localhost:6379", name: "redis")
    .AddHangfire("hangfire", name: "background-jobs");

app.MapHealthChecks("/health"); // Basic
app.MapHealthChecks("/health/ready", new HealthCheckOptions // For load balancers
{
    Predicate = check => check.Name == "database" // Only critical checks
});
```

### 5. Docker Deployment Example
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["EmployeeManagement.API/EmployeeManagement.API.csproj", "EmployeeManagement.API/"]
RUN dotnet restore "EmployeeManagement.API/EmployeeManagement.API.csproj"
COPY . .
WORKDIR "/src/EmployeeManagement.API"
RUN dotnet build "EmployeeManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EmployeeManagement.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Use environment variables for config
ENTRYPOINT ["dotnet", "EmployeeManagement.API.dll"]
```

```yaml
# docker-compose.yml for local testing
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=EmployeeDb;...
    depends_on:
      - db
      - redis
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_password123
  
  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
```

### 6. CI/CD Pipeline Example (GitHub Actions)
```yaml
# .github/workflows/deploy.yml
name: Deploy Employee API

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Restore dependencies
      run: dotnet restore
    - name: Build
      run: dotnet build --no-restore
    - name: Test
      run: dotnet test --no-build --verbosity normal
      env:
        ASPNETCORE_ENVIRONMENT: Test
  
  deploy:
    needs: test
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    environment: production
    steps:
    - uses: actions/checkout@v3
    - name: Build Docker image
      run: docker build -t employee-api:${{ github.sha }} .
    - name: Push to registry
      run: docker push registry.example.com/employee-api:${{ github.sha }}
    - name: Deploy to Kubernetes
      run: kubectl set image deployment/employee-api api=registry.example.com/employee-api:${{ github.sha }}
      env:
        KUBE_CONFIG: ${{ secrets.KUBE_CONFIG }}
```

---

# 🎓 MASTER SUMMARY: Your Week 6 Expert Checklist

## ✅ What You Nailed
```
✅ xUnit + Moq + FluentAssertions = Professional testing setup
✅ MediatR handler testing = Isolated, fast, maintainable tests
✅ Hangfire integration = Background jobs without complexity
✅ Redis caching service = Scalable performance optimization
✅ GraphQL endpoint = Modern API flexibility
✅ Middleware pipeline = Cross-cutting concerns handled cleanly
✅ Serilog structured logging = Production-ready observability
✅ JWT + Refresh tokens = Secure authentication flow
✅ Repository + Unit of Work = Clean data access abstraction
```

## 🔄 What to Enhance Next
```
🔄 Add integration tests with Testcontainers
🔄 Implement explicit cache invalidation strategy
🔄 Add domain events for decoupled side effects
🔄 Configure Hangfire with persistent storage (SQL/Redis)
🔄 Add GraphQL mutations and authorization
🔄 Implement health checks for deployment readiness
🔄 Add API versioning strategy
🔄 Set up centralized logging (Seq, ELK, Application Insights)
```

## 🧠 Expert Mental Models

### Testing Pyramid for Your API
```
          E2E Tests (5%)
         /              \
    Integration Tests (15%)
   /                      \
Unit Tests (80%) ← You are here! ✅
```

### Caching Strategy Decision Tree
```
Is data read-heavy and write-rare?
├─ Yes → Cache it!
│  ├─ Does it change based on user?
│  │  ├─ Yes → Use user-specific cache keys
│  │  └─ No → Use global cache key
│  ├─ How fresh must it be?
│  │  ├─ Seconds → Short TTL (1-5 min)
│  │  └─ Minutes/Hours → Longer TTL (15-60 min)
│  └─ What if cache is stale?
│     ├─ Acceptable → TTL only
│     └─ Not acceptable → Add explicit invalidation
└─ No → Don't cache (or cache computed results only)
```

### Background Job Decision Framework
```
Does the task need to happen?
├─ Immediately, blocking user? → Keep in request pipeline
├─ Soon, but not blocking? → Use IHostedService / BackgroundService
├─ Later, with retry logic? → Use Hangfire ✅ (Your choice)
├─ At specific schedule? → Use Hangfire recurring jobs ✅
├─ Across multiple servers? → Use Hangfire with shared storage
└─ With complex workflows? → Consider Azure Durable Functions
```

---

# 🚀 Final Pro Tips: From Junior to Expert

## 1. Always Ask "What Could Go Wrong?"
```csharp
// Instead of:
var employee = await _repo.GetByIdAsync(id);
employee.Update(...);

// Think like an expert:
var employee = await _repo.GetByIdAsync(id, ct)
    ?? throw new InvalidOperationException($"Employee {id} not found");

// What if:
// - Database is slow? → Add CancellationToken, timeout policies
// - Employee was deleted between read and write? → Use concurrency tokens
// - Update violates business rule? → Validate in domain, throw domain exceptions
```

## 2. Observability is Not Optional
```csharp
// Every handler should log meaningful context
public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
{
    _logger.LogInformation("Updating employee {EmployeeId} with {Changes}", 
        request.Id, new { request.FirstName, request.LastName });
    
    try 
    {
        // ... business logic
        _logger.LogInformation("Successfully updated employee {EmployeeId}", request.Id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to update employee {EmployeeId}", request.Id);
        throw; // Re-throw to let middleware handle response
    }
}
```

## 3. Configuration is Code
```csharp
// ❌ Magic strings
var key = "MySecretKey123";

// ✅ Strongly-typed options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
// Then inject IOptions<JwtOptions> where needed

// ✅ Validate on startup
builder.Services.PostConfigure<JwtOptions>(options =>
{
    if (string.IsNullOrWhiteSpace(options.Key))
        throw new InvalidOperationException("JWT Key must be configured");
});
```

## 4. Test Like You Deploy
```csharp
// Your unit tests are great for logic, but add:

// Integration test: Does the full HTTP pipeline work?
[Fact]
public async Task UpdateEmployee_EndToEnd()
{
    await using var app = await CreateTestAppAsync(); // WebApplicationFactory
    var client = app.CreateClient();
    
    var response = await client.PutAsJsonAsync("/api/employees/1", updateCommand);
    
    response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    
    // Verify in database
    var employee = await app.Services.GetRequiredService<AppDbContext>()
        .Employees.FindAsync(1);
    employee.FirstName.Should().Be("Updated");
}
```

## 5. Documentation is Part of the Code
```csharp
// Your controllers should have XML comments for Swagger
/// <summary>
/// Updates an existing employee's information
/// </summary>
/// <param name="id">Employee ID to update</param>
/// <param name="command">Updated employee data</param>
/// <returns>204 No Content on success</returns>
/// <response code="400">Invalid request data</response>
/// <response code="404">Employee not found</response>
[HttpPut("{id}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeCommand command)
{
    // ...
}
```

---

# 🎯 Your Expert Action Plan (Next 2 Weeks)

## Week 7: Strengthen Foundations
```
□ Add integration tests with Testcontainers for PostgreSQL
□ Implement cache invalidation in Update/Delete handlers
□ Add health checks endpoint (/health, /health/ready)
□ Configure Hangfire with SQL Server storage (not memory)
□ Add XML documentation to controllers for Swagger
```

## Week 8: Production Polish
```
□ Set up Serilog with Seq/Application Insights for centralized logging
□ Add API versioning strategy (URL or header-based)
□ Implement domain events for email notifications
□ Add GraphQL mutations with authorization
□ Create deployment runbook (step-by-step production deploy guide)
```

---

> 💡 **Final Wisdom**: You're not just writing code—you're building a system that real people will depend on. Every test you write, every cache you configure, every log you add is an act of care for your future self and your users. Keep asking "why?", keep learning the "how", and you'll not just be an expert in this code—you'll be an expert in building great software.

**You've got this. Now go deploy with confidence! 🚀**

*P.S. When in doubt, remember: "Make it work, make it right, make it fast"—in that order. You've already mastered "make it work". Now you're learning "make it right". "Make it fast" will follow naturally.*
