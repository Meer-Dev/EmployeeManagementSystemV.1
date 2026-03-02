Client (HTTP Request)

       │

       ▼

[Middleware Pipeline]

 ├─ RequestLoggingMiddleware

 │     Logs request details (method, path, status, elapsed ms) using Serilog.

 ├─ ExceptionMiddleware

 │     Catches exceptions globally and logs them (Serilog + database via `IExceptionLogger`).

 ├─ ValidationBehavior (FluentValidation)

 │     Validates incoming DTO/command in the MediatR pipeline.

 │

 ▼

[Controller Layer]

 └─ EmployeesController (thin API layer – uses IMediator only)

       │

       ▼

[MediatR / Application Layer]

 ├─ Commands (Create, Update, Delete, Patch)

 │     Executes business logic using domain entities.

 ├─ Queries (GetEmployeeById, GetEmployees)

 │     Executes read logic with optional pagination/filtering/sorting.

 │

 ▼

[Infrastructure Layer]

 └─ EF Core / Database Operations (AppDbContext, DatabaseExceptionLogger)

       │

       ▼

[Response Generation]

 └─ ApiResponse-style JSON (success flag, message, data or errors)

       │

       ▼

[Middleware Pipeline]

 └─ Response Logging (Serilog sinks – console + rolling files)

       │

       ▼

Client receives standardized response
--- 

✅ High-Level Roadmap

When creating a full enterprise API from zero:

1️⃣ Start With the Solution Structure
    You create the empty projects first.

2️⃣ Add the Domain Layer
    Because everything depends on your core entities.

3️⃣ Add the Application Layer (CQRS + MediatR + Validation + Mapping)

4️⃣ Add the Infrastructure Layer (EF Core + DB + Auth Providers)

5️⃣ Add the API Layer
    Controllers, middleware, logging, DI registration, request pipeline.

6️⃣ Add Authentication
    JWT or Identity.

7️⃣ Add Cross-Cutting Concerns
    Exception middleware
    Logging middleware
    ApiResponse wrapper

8️⃣ Run migration → Test basic endpoints → Add more features

✅ Summary

1️⃣ Create Solution Structure (Domain → Application → Infrastructure → API)
2️⃣ Create Domain Entities (Employee etc.)
3️⃣ Create Application Layer (CQRS commands/queries)
4️⃣ Create Infrastructure DB + EF + Auth
5️⃣ Create API Controllers + Middleware + Logging
6️⃣ Add Authentication + Authorization
7️⃣ Add cross-cutting concerns (mapping, validation)
8️⃣ Run Migrations → Test API

---

## Project Structure (Clean Architecture)

- **EmployeeManagement.Domain**: Core domain entities and rules (e.g. `Employee` with `Activate`, `Deactivate`, `Update` methods).
- **EmployeeManagement.Application**: CQRS commands/queries + handlers, validation pipeline, mappings, and abstractions like `IReadAppDbContext`, `IWriteAppDbContext`, `IExceptionLogger`.
- **EmployeeManagement.Infrastructure**: EF Core `AppDbContext`, migrations, and implementations of application interfaces (including `DatabaseExceptionLogger` writing into `ExceptionLogs` table). Registered via `AddInfrastructure`.
- **EmployeeManagement.API**: Thin HTTP layer (controllers, middleware, filters, Serilog configuration) that depends only on Application and Infrastructure DI extensions.

This keeps dependencies pointing **inward** (API → Application → Domain; Infrastructure → Application abstractions), following Clean Architecture.

## Week 4 – Deep Dive Notes (Validations, Middleware, Logging, Pagination, Third‑Party Libraries)

This section documents **exactly what was implemented in Week 4**, why it exists, when it runs in the request pipeline, and common **alternatives** you could mention in an interview or viva.

### 1. Validations with FluentValidation + MediatR Pipeline

#### 1.1. Where validation lives

- **`CreateEmployeeValidator`** (`EmployeeManagement.Application.Employees.Commands.CreateEmployee.CreateEmployeeValidator`)
  - Defines business rules for creating an employee.
  - Uses **FluentValidation** to validate the `CreateEmployeeCommand`.
- **`ValidationBehavior<TRequest, TResponse>`** (`EmployeeManagement.Application.Common.Behaviors.ValidationBehavior`)
  - A **MediatR pipeline behavior** that runs **before** any command/query handler.
  - Collects all validators for a request type and executes them.
  - Throws `FluentValidation.ValidationException` if rules fail.
- **Registration** (`EmployeeManagement.Application.DependencyInjection`)
  - `services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());`
    - Scans the Application assembly and auto‑registers all classes inheriting from `AbstractValidator<T>`.
  - `services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));`
    - Adds the validation behavior into the MediatR pipeline so **every** request goes through validation first.

#### 1.2. How the validation flow works

1. Client sends a request (e.g. `POST /api/employees`) with JSON body.
2. ASP.NET Core model binding converts JSON into `CreateEmployeeCommand`.
3. Controller action calls `_mediator.Send(command)`.
4. MediatR builds a **pipeline**:
   - `ValidationBehavior<CreateEmployeeCommand, int>` runs first.
   - If validation passes, control moves to `CreateEmployeeCommandHandler`.
5. `ValidationBehavior`:
   - Looks up all `IValidator<CreateEmployeeCommand>` implementations (in our case `CreateEmployeeValidator`).
   - Executes them and gathers all `ValidationFailure`s.
   - If any failures exist, throws `ValidationException` **before handler runs**.
6. `ExceptionMiddleware` catches the `ValidationException` and returns a **400 Bad Request** with a clean JSON error response.

#### 1.3. Why FluentValidation + pipeline behavior (pros)

- **Separation of concerns**: Handlers focus only on business logic. Validation rules are in dedicated validator classes.
- **Reusability**: You can reuse the same validator in multiple places if needed.
- **Consistency**: All MediatR requests are validated the same way (via `ValidationBehavior`).
- **Testability**: You can unit‑test validators independently from handlers.

#### 1.4. When to use this approach

- When you adopt **CQRS + MediatR** and expect many commands/queries.
- When your domain has **rich validation rules** and you want them clearly organized.
- When the team prefers **fluent, readable validation syntax**.

#### 1.5. Alternatives you could mention

- **Data Annotations**:
  - Use attributes like `[Required]`, `[MaxLength]`, `[EmailAddress]` directly on DTO/command properties.
  - Simpler for small projects, but can become messy for complex rules.
- **Manual validation in controllers/handlers**:
  - `if (string.IsNullOrEmpty(firstName)) return BadRequest(...);`
  - Flexible but duplicates logic and mixes concerns.
- **Other validation libraries**:
  - E.g. `DataAnnotations` + custom `IValidatableObject`, or custom middleware.

### 2. Middleware, Centralized Exception Handling, and Request Logging

#### 2.1. Where middleware is configured

- **`Program.cs`**:
  - `app.UseMiddleware<ExceptionMiddleware>();`
  - `app.UseMiddleware<RequestLoggingMiddleware>();`
  - These are inserted early in the **HTTP pipeline** so that:
    - `ExceptionMiddleware` can see and handle exceptions from downstream.
    - `RequestLoggingMiddleware` can measure the full elapsed time of the request.

#### 2.2. `ExceptionMiddleware` (centralized error handling)

- Location: `EmployeeManagement.API.Middleware.ExceptionMiddleware`.
- Purpose:
  - Wraps the rest of the pipeline in a `try/catch`.
  - Translates **unhandled exceptions** into **standardized JSON error responses**.
  - Logs errors using `ILogger<ExceptionMiddleware>`.
- Behavior:
  - On `ValidationException`:
    - Logs: `"Validation error: {message}"`.
    - Returns `400 BadRequest` with body containing:
      - `Success = false`
      - `Message = "Validation error"`
      - `Errors = [ { PropertyName, ErrorMessage }, ... ]`
  - On any other `Exception`:
    - Logs: `"An unexpected error occurred: {message}"`.
    - Returns `500 InternalServerError` with generic message (no stack trace leakage).

#### 2.3. `RequestLoggingMiddleware` (request/response logging + timing)

- Location: `EmployeeManagement.API.Middleware.RequestLoggingMiddleware`.
- Purpose:
  - Measure API performance and log each HTTP call.
  - Useful for debugging, monitoring, and performance analysis.
- Behavior:
  - Starts a `Stopwatch` **before** calling `_next(context)`.
  - Executes the rest of pipeline (controllers, handlers, DB, etc.).
  - Stops stopwatch, then logs:
    - HTTP method (GET/POST/PUT/DELETE/PATCH).
    - Request path (`/api/employees`).
    - Response status code (200/400/500, etc.).
    - Elapsed time in milliseconds.

#### 2.4. Database exception logging via `IExceptionLogger`

- Interface: `IExceptionLogger` (`EmployeeManagement.Application.Common.Interfaces.IExceptionLogger`).
- Implementation: `DatabaseExceptionLogger` in Infrastructure, registered in `AddInfrastructure`.
- `ExceptionMiddleware` resolves `IExceptionLogger` from the current request scope and calls:
  - `LogAsync(exception, path, method, statusCode, cancellationToken)`.
- Under the hood, exceptions (including validation failures) are persisted to the `ExceptionLogs` table via EF Core, giving you a durable error history in addition to Serilog logs.

#### 2.5. Why custom middleware (pros)

- **Single place** for error handling and logging, instead of repeating in each controller.
- Can be extended later (e.g. correlation IDs, user info, response body logging).
- Works for **all endpoints** without modifying controller code.

#### 2.6. Alternatives for exception handling and logging

- **ASP.NET Core built‑in exception handler**:
  - `app.UseExceptionHandler("/error");` with an error controller.
  - Simpler but less customized than our middleware.
- **Filters (ExceptionFilter / ActionFilter)**:
  - Good for MVC but middleware is more general (works for everything in the pipeline).
- **Third‑party middleware**:
  - Example: libraries for problem‑details (`RFC 7807`) responses.

### 3. Serilog – Third‑Party Logging Library

#### 3.1. Where Serilog is configured

- **`Program.cs`**:
  - `Log.Logger = new LoggerConfiguration()...`
    - Adds **console** sink: logs visible in terminal.
    - Adds **file** sink: `logs/log-.txt` with daily rolling files.
  - `builder.Host.UseSerilog();`
    - Replaces default `ILogger` implementation with Serilog.
- **`appsettings.json` / `appsettings.Development.json`**:
  - Currently only standard logging levels (`Logging:LogLevel`).
  - Could be extended to configure Serilog via configuration instead of code.

#### 3.2. Why Serilog (pros)

- **Structured logging**:
  - Logs can be written with named properties, e.g. `@UserId`, `@ElapsedMilliseconds`, which work well with search tools.
- **Multiple sinks**:
  - Console, file, Seq, ELK stack, Application Insights, etc.
- **Better querying and dashboards** when combined with log storage tools.

#### 3.3. Alternatives you can mention

- **Built‑in `ILogger` only**:
  - Simpler but less powerful for structured logging.
- **NLog**, **log4net**, **Microsoft.Extensions.Logging** with other providers.

### 4. AutoMapper – Mapping between Entity and DTO

#### 4.1. Where AutoMapper is configured

- **Profile**: `EmployeeProfile` (`EmployeeManagement.Application.Common.Mappings.EmployeeProfile`)
  - Inherits from `Profile`.
  - Defines `CreateMap<Employee, EmployeeDto>();` so AutoMapper knows how to map.
- **Registration**: `EmployeeManagement.Application.DependencyInjection`
  - `services.AddAutoMapper(Assembly.GetExecutingAssembly());`
  - Scans Application assembly for profiles and registers them.

#### 4.2. Why use AutoMapper here

- Avoids **manual property copying** between `Employee` and `EmployeeDto`.
- Centralizes mapping rules; if the domain changes, you update mappings in one place.
- Encourages a **clean separation** between domain entities and DTOs.

#### 4.3. Alternatives

- **Manual mapping**:
  - `new EmployeeDto(e.Id, e.FirstName, ...)` as done in `GetEmployeesQueryHandler`.
  - More explicit and type‑safe, but more repetitive.
- **Mapster** or other mapping libraries.

### 5. Pagination, Filtering, Sorting

#### 5.1. Core types

- **`PagedResult<T>`** (`EmployeeManagement.Application.Common.PagedResult<T>`)
  - Represents a **paged set of results**:
    - `Items`: the current page of items (`IEnumerable<T>`).
    - `TotalCount`: how many records exist in total for this query.
- **`GetEmployeesQuery`** (`EmployeeManagement.Application.Employees.Queries.GetEmployees.GetEmployeesQuery`)
  - Defines query inputs from the client:
    - `PageNumber` (default `1`).
    - `PageSize` (default `100`).
    - `Search` (for filtering by `FirstName`).
    - `SortBy` (e.g. `"name"` or default by Id).
- **`GetEmployeesQueryHandler`**
  - Implements EF Core LINQ to apply filtering, sorting, pagination, and projection into `EmployeeDto`.

#### 5.2. How the flow works

1. Client calls `GET /api/employees?pageNumber=1&pageSize=10&search=Ali&sortBy=name`.
2. `EmployeesController.Get` action receives `GetEmployeesQuery` via `[FromQuery]`.
3. `_mediator.Send(query)` is called.
4. `GetEmployeesQueryHandler`:
   - Starts from `_context.Employees.AsQueryable()`.
   - **Filtering**:
     - If `Search` is not null/empty, adds a `Where` on `FirstName`.
   - **Sorting**:
     - Uses C# `switch` expression on `SortBy`.
     - `"name"` → order by `FirstName`.
     - Default (`_`) → order by `Id`.
   - **Total count**:
     - `CountAsync` on the filtered/sorted query.
   - **Paging**:
     - Uses `Skip((PageNumber - 1) * PageSize)` and `Take(PageSize)`.
   - **Projection**:
     - Projects each `Employee` into `EmployeeDto`.
   - Wraps everything into `new PagedResult<EmployeeDto> { Items = items, TotalCount = total }`.

#### 5.3. Why this approach (pros)

- Efficient: uses **IQueryable** and translates to SQL on the server side.
- Flexible: easy to extend to more filters/sorts later.
- Standard pattern: many APIs use `pageNumber`, `pageSize`, and `totalCount`.

#### 5.4. Alternatives to mention

- **Cursor‑based pagination**:
  - Use a cursor/offset token instead of page numbers (good for large data/real‑time feeds).
- **Offset/limit only**:
  - Client passes `offset` and `limit` instead of `pageNumber` and `pageSize`.
- **OData** or other query languages**:
  - Clients can send filters/sorts in a richer query syntax.

### 6. API Response Standardization

#### 6.1. `ApiResponse`

- Location: `EmployeeManagement.API.Common.ApiResponse`
- Structure:
  - `Success`: indicates if operation succeeded.
  - `Message`: human‑readable description.
  - `Data`: any object (payload, list, single item, etc.).
- Intention:
  - To standardize **all API responses** so clients always see a consistent shape:
    - On success: `{ success, message, data }`
    - On error: `{ success: false, message, errors? }`
- Currently:
  - `ExceptionMiddleware` constructs anonymous objects with same idea.
  - Next step (future improvement) is to **replace raw `Ok(...)`/`CreatedAtAction`** with wrappers that return `ApiResponse`.

#### 6.2. Alternatives

- **Plain data responses**:
  - Return `EmployeeDto` or `PagedResult<EmployeeDto>` directly.
- **ProblemDetails (RFC 7807)**:
  - Use standardized error format with `type`, `title`, `status`, `detail`.
- **Custom envelope per project**:
  - E.g. `{ statusCode, isSuccess, result, error }`.

### 7. CQRS + MediatR Layering (How controllers talk to the rest of the system)

#### 7.1. Controllers

- `EmployeesController` uses **MediatR** (`IMediator`) and does **not** access EF Core directly.
- Each HTTP verb is mapped to a corresponding **command or query**:
  - `POST /api/employees` → `CreateEmployeeCommand`
  - `PUT /api/employees/{id}` → `UpdateEmployeeCommand`
  - `PATCH /api/employees/{id}` → `PatchEmployeeCommand`
  - `DELETE /api/employees/{id}` → `DeleteEmployeeCommand`
  - `GET /api/employees` → `GetEmployeesQuery`
  - `GET /api/employees/{id}` → `GetEmployeeByIdQuery`

#### 7.2. Application layer handlers and entities

- **Commands**:
  - `CreateEmployeeCommandHandler`
  - `UpdateEmployeeCommandHandler`
  - `DeleteEmployeeCommandHandler`
  - `PatchEmployeeCommandHandler`
- **Queries**:
  - `GetEmployeesQueryHandler`
  - `GetEmployeeByIdQueryHandler`
- **Domain entity**:
  - `Employee` encapsulates domain rules (`Activate`, `Deactivate`, `Update...`).
- **DTO**:
  - `EmployeeDto` is used to return data to the API layer without exposing the entity directly.

### 8. Infrastructure Layer – EF Core and Context Interfaces

#### 8.1. `AppDbContext`

- Implements both `IReadAppDbContext` and `IWriteAppDbContext`.
- Exposes:
  - `DbSet<Employee> Employees` and `DbSet<ExceptionLog> ExceptionLogs` for tracking and saving.
- The read interface (`IReadAppDbContext`) exposes `IQueryable<Employee> Employees`, which handlers shape using LINQ; EF Core translates this into SQL.
- The write interface (`IWriteAppDbContext`) exposes methods `AddEmployee`, `GetEmployeeByIdAsync`, `DeleteEmployee`, `SaveChangesAsync`.

#### 8.2. Why split read/write interfaces

- Promotes a **CQRS mindset**:
  - Queries depend only on `IReadAppDbContext`.
  - Commands depend only on `IWriteAppDbContext`.
- Makes unit testing easier – you can mock different interfaces depending on use‑case.

#### 8.3. Registration in Infrastructure DI

- `EmployeeManagement.Infrastructure.DependencyInjection`:
  - Configures EF Core with SQL Server using the `"DefaultConnection"` string.
  - Registers `AppDbContext` as the implementation for both read and write interfaces.

---

With these notes and the per‑line comments inside the Week 4 files, you can confidently explain:

- **What** each file does (purpose).
- **Where** it sits in the pipeline or layers.
- **Why** this approach was chosen.
- **When** it runs during a request.
- **What alternatives** you know and when to use them.
