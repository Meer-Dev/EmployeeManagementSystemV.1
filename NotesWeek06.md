# Comprehensive Software Architecture and Development Notes
*Study Module for .NET Engineers | Interview Prep & Production Design*

---

## Table of Contents
1. [Domain Events](#domain-events)
2. [OAuth 2.0](#oauth-20)
3. [Background Jobs](#background-jobs)
4. [CORS (Cross-Origin Resource Sharing)](#cors-cross-origin-resource-sharing)
5. [Redis](#redis)
6. [JWT (JSON Web Tokens)](#jwt-json-web-tokens)
7. [Unit Testing (xUnit) & Mocking (Moq)](#unit-testing-xunit--mocking-moq)
8. [GraphQL](#graphql)
9. [FluentValidation](#fluentvalidation)
10. [Serilog](#serilog)
11. [AutoMapper](#automapper)
12. [Middleware & Custom Middleware](#middleware--custom-middleware)
13. [Request/Response Logging](#requestresponse-logging)
14. [CQRS](#cqrs)
15. [Repository Pattern & Unit of Work](#repository-pattern--unit-of-work)
16. [MediatR, Handlers, Request/Response Models](#mediatr-handlers-requestresponse-models)
17. [Entity Framework Core & DbContext](#entity-framework-core--dbcontext)

---

## Domain Events

### Overview
**Purpose**: Decouple business logic by allowing domain objects to raise events that other parts of the system can react to without direct dependencies.

# Enhanced Software Architecture & Development Notes
## **Employee Management Portal Edition**
*Study Module for .NET Engineers | Interview Prep & Production Design*

---

## **Table of Contents**
1. [Domain Events](#1-domain-events)
2. [OAuth 2.0](#2-oauth-20)
3. [Background Jobs](#3-background-jobs)
4. [CORS (Cross-Origin Resource Sharing)](#4-cors-cross-origin-resource-sharing)
5. [Redis](#5-redis)
6. [JWT (JSON Web Tokens)](#6-jwt-json-web-tokens)
7. [Unit Testing (xUnit) & Mocking (Moq)](#7-unit-testing-xunit--mocking-moq)
8. [GraphQL](#8-graphql)
9. [FluentValidation](#9-fluentvalidation)
10. [Serilog](#10-serilog)
11. [AutoMapper](#11-automapper)
12. [Middleware & Custom Middleware](#12-middleware--custom-middleware)
13. [Request/Response Logging](#13-requestresponse-logging)
14. [CQRS](#14-cqrs)
15. [Repository Pattern & Unit of Work](#15-repository-pattern--unit-of-work)
16. [MediatR, Handlers, Request/Response Models](#16-mediatr-handlers-requestresponse-models)
17. [Entity Framework Core & DbContext](#17-entity-framework-core--dbcontext)

---

## 1. Domain Events

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Direct method calls inside business logic.
    *   *Example:* When saving a new `Employee`, the code directly called `EmailService.SendWelcomeEmail()` and `ITService.ProvisionLaptop()` inside the `Save()` method.
      
*   **The Problem:**
    *   **Tight Coupling:** The `Employee` class knew about Email and IT services.
    *   **Slow Performance:** Saving an employee waited for emails to send.
    *   **Hard to Test:** Couldn't test employee creation without mocking email servers.
    *   **Rigidity:** Adding a new step (e.g., "Add to Slack") required changing the core `Employee` class.

### **The "After" State (Domain Events)**
*   **The Fix:** The `Employee` entity raises a generic event (`EmployeeCreatedEvent`). Separate handlers listen for this event to perform side effects.
*   **Employee Portal Scenario:**
    *   **Event:** `EmployeeOnboardedEvent`
    *   **Handlers:**
        1.  `SendWelcomeEmailHandler`
        2.  `ProvisionHardwareHandler`
        3.  `AddToPayrollSystemHandler`

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Direct Calls (Before)** | Simple to debug, immediate consistency | Tight coupling, slow main thread, hard to scale |
| **Domain Events (After)** | Decoupled, scalable, easier to add new features | Eventual consistency (data might lag), harder to trace flow |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Imagine you join a school club. The president has to personally call the IT guy for your ID card, the librarian for your books, and the cafeteria for your meal plan. If the president is busy, you wait.
    *   **After:** You ring a bell ("I'm new!"). Different people hear the bell and do their jobs automatically. The president doesn't need to know who does what.
*   **👨‍💻 For the Senior Architect:**
    *   **Focus:** Decoupling aggregate boundaries.
    *   **Consistency:** Acknowledge **Eventual Consistency**. The employee exists in the DB before the email is sent.
    *   **Reliability:** Implement the **Outbox Pattern** to ensure events aren't lost if the app crashes between DB commit and event publish.
    *   **Complexity:** Introduces distributed tracing challenges; ensure `CorrelationId` flows through events.

**How it works**: 
- Domain entities raise events when state changes occur
- Event handlers subscribe to and process these events
- Events are simple POCOs implementing a marker interface

**When to use**: 
- Complex business workflows requiring multiple side effects
- Maintaining aggregate consistency boundaries
- Event-driven architectures, microservices communication

```csharp
// Domain Event Interface
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

// Concrete Event
public class OrderPlacedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public decimal TotalAmount { get; }
    
    public OrderPlacedEvent(Guid orderId, decimal totalAmount)
    {
        OrderId = orderId;
        TotalAmount = totalAmount;
    }
}

// Entity raising event
public class Order : AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void PlaceOrder()
    {
        // ... business logic ...
        _domainEvents.Add(new OrderPlacedEvent(this.Id, this.Total));
    }
    
    public void ClearEvents() => _domainEvents.Clear();
}
```

### Architecture Workflow
```
┌─────────────┐     ┌──────────────────┐     ┌─────────────────┐
│  Domain     │     │  Event Dispatcher│     │  Event Handlers │
│  Entity     │────▶│  (MediatR/Custom)│────▶│  - EmailService │
│  Raises     │     │                  │     │  - InventorySvc │
│  Event      │     │                  │     │  - Analytics    │
└─────────────┘     └──────────────────┘     └─────────────────┘
```

### Real-World Use Cases
- **E-commerce**: Order placed → send confirmation, update inventory, trigger fulfillment
- **Banking**: Transaction completed → update ledger, send notification, fraud check
- **SaaS**: User subscribed → provision resources, send welcome email, update billing

### Trade-offs & Alternatives
| Approach | Pros | Cons |
|----------|------|------|
| **In-process events** | Simple, fast, transactional | Tightly coupled, single process |
| **Message broker (RabbitMQ)** | Decoupled, scalable, resilient | Complexity, eventual consistency |
| **Outbox pattern** | Reliable delivery, transactional | Additional storage, polling overhead |

### Internal Process: Event Propagation
1. Aggregate modifies state and adds event to internal list
2. Application service commits transaction, then publishes events
3. Event dispatcher resolves handlers via DI container
4. Handlers execute sequentially or in parallel
5. Failed handlers: retry policies, dead-letter queues

### Tools & Libraries
- **MediatR**: In-process mediator for event publishing (`IPublisher.Publish()`)
- **MassTransit/NServiceBus**: For distributed event handling
- **EF Core SaveChanges override**: Capture domain events pre/post save

### Best Practices
✅ Publish events *after* transaction commits to avoid inconsistent state  
✅ Keep events immutable and descriptive (past tense: `OrderShipped`)  
✅ Include only necessary data; avoid lazy-loading traps  
✅ Handle idempotency in event handlers  

### Common Pitfalls
❌ Raising events during entity construction (partial state)  
❌ Publishing events inside database transactions (distributed locking)  
❌ Overusing events for simple CRUD (YAGNI)  

### Interview Insights
> **Q**: "How do you ensure domain events are not lost if the app crashes after saving but before publishing?"  
> **A**: Use the **Outbox Pattern**: Store events in the same transaction as business data, then have a background job reliably publish them.

---

## OAuth 2.0

### Overview
**Purpose**: Delegated authorization framework allowing third-party apps to access user resources without exposing credentials.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Basic Authentication (Username/Password sent with every request) or custom session cookies stored in server memory.
*   **The Problem:**
    *   **Security Risk:** Passwords transmitted frequently; server memory sessions don't scale across multiple servers.
    *   **Integration Hell:** Hard to let third-party apps (like a mobile attendance app) access data safely.

### **The "After" State (OAuth 2.0)**
*   **The Fix:** Delegated authorization using tokens. The Employee Portal trusts an Identity Provider (like Azure AD or Auth0).
*   **Employee Portal Scenario:**
    *   **Flow:** Employee logs in via Corporate SSO → Portal receives Access Token → Portal uses token to call HR API.
    *   **Scope:** Token allows `read:profile` but not `write:salary`.

### **Trade-offs**
| Strategy | Pros | Cons |
|----------|------|------|
| **Session Cookies (Before)** | Easy to revoke, simple | Sticky sessions required, hard to scale horizontally |
| **OAuth/JWT (After)** | Stateless, scalable, secure delegation | Harder to revoke tokens immediately, complex setup |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Showing your password to every shop you visit to prove who you are.
    *   **After:** Getting a wristband at a theme park. You show the wristband (token) to ride rides (APIs). The wristband expires after a day, and you don't share your home address (password).
*   **👨‍💻 For the Senior Architect:**
    *   **Security:** Enforce **PKCE** for public clients (mobile/SPA).
    *   **Revocation:** JWTs are stateless; implement a **Refresh Token Rotation** strategy or a token blacklist in Redis for immediate revocation on employee termination.
    *   **Scopes:** Apply **Least Privilege**. A "Manager" token should not have "Admin" scopes.


**Key Flows**:
| Flow | Use Case | Security Level |
|------|----------|---------------|
| Authorization Code | Web apps with backend | ★★★★★ |
| Authorization Code + PKCE | Mobile/SPA apps | ★★★★☆ |
| Client Credentials | Service-to-service | ★★★☆☆ |
| Resource Owner Password | Legacy/trusted apps | ★★☆☆☆ (avoid) |

### Authorization Code Flow (with PKCE)
```
1. Client → Auth Server: 
   GET /authorize?response_type=code&client_id=xyz&redirect_uri=...&code_challenge=...

2. User authenticates & consents

3. Auth Server → Client: 
   Redirect to redirect_uri?code=AUTH_CODE

4. Client → Auth Server: 
   POST /token 
   { grant_type: "authorization_code", code: AUTH_CODE, code_verifier: "...", client_secret: "..." }

5. Auth Server → Client: 
   { access_token: "eyJ...", refresh_token: "...", expires_in: 3600 }
```

### C# Implementation (ASP.NET Core)
```csharp
// Startup.cs - Add authentication
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect("Auth0", options =>
{
    options.Authority = "https://your-domain.auth0.com";
    options.ClientId = "your-client-id";
    options.ClientSecret = "your-client-secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid profile email");
});

// Protecting an API endpoint
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<Order> GetOrder(Guid id)
    {
        var userId = User.FindFirst("sub")?.Value;
        // ... fetch order scoped to userId
    }
}
```

### Token Exchange & Refresh
```csharp
public async Task<TokenResponse> RefreshAccessTokenAsync(string refreshToken)
{
    var client = _httpClientFactory.CreateClient();
    var response = await client.PostAsync("https://auth-server/token", 
        new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "refresh_token"),
            new KeyValuePair<string, string>("refresh_token", refreshToken),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret)
        }));
    
    return await response.Content.ReadFromJsonAsync<TokenResponse>();
}
```

### Real-World Considerations
- **Token storage**: HttpOnly cookies for web; secure storage for mobile
- **Short-lived access tokens** (15-60 min) + **refresh tokens** (days/weeks)
- **Scope granularity**: `read:orders`, `write:products` for least privilege
- **Revocation**: Maintain refresh token blacklist or use reference tokens

### Trade-offs
| Strategy | Pros | Cons |
|----------|------|------|
| **JWT (self-contained)** | Stateless, scalable | Can't revoke easily, large payload |
| **Reference tokens** | Revocable, small | Requires token introspection endpoint |
| **Backend-for-frontend (BFF)** | Secure cookie handling | Additional service layer |

### Best Practices
✅ Always use HTTPS  
✅ Validate `iss`, `aud`, `exp` claims manually if not using middleware  
✅ Implement token rotation for refresh tokens  
✅ Use PKCE for public clients (mobile/SPA)  

### Common Pitfalls
❌ Storing tokens in localStorage (XSS vulnerability)  
❌ Not validating token signature or issuer  
❌ Using resource owner password flow for third-party apps  

### Interview Insights
> **Q**: "How would you handle token expiration in a SPA?"  
> **A**: Use an interceptor (Axios, Fetch wrapper) to catch 401s, silently refresh using a stored refresh token via a secure BFF endpoint, then retry the original request. Implement exponential backoff for retry logic.

**References**: 
- [OAuth 2.1 Draft](https://oauth.net/2.1/)
- [Duende IdentityServer](https://duendesoftware.com/)
- [Auth0 .NET SDK](https://github.com/auth0/auth0.net)

---

## Background Jobs

### Overview
**Purpose**: Execute long-running, delayed, or recurring tasks outside the HTTP request pipeline.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Windows Task Scheduler triggering a console app, or `System.Timers.Timer` inside the web app.
*   **The Problem:**
    *   **Unreliable:** If the web app restarts, in-memory timers die.
    *   **No Visibility:** No dashboard to see if payroll processing failed.
    *   **Blocking:** Long tasks blocked the web server threads.

### **The "After" State (Background Jobs)**
*   **The Fix:** Persistent job queues (Hangfire/Azure Functions) that survive restarts and offer retry logic.
*   **Employee Portal Scenario:**
    *   **Job:** `MonthlyPayrollProcessingJob`.
    *   **Trigger:** Runs automatically on the 25th of every month.
    *   **Retry:** If the payment gateway fails, retry 3 times before alerting HR.

### **Trade-offs**
| Tool | Pros | Cons |
|------|------|------|
| **In-Memory Timer (Before)** | Zero setup, fast | Lost on restart, no persistence, single server only |
| **Hangfire/Queue (After)** | Persistent, retry logic, dashboard | External dependency (SQL/Redis), operational overhead |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Setting an alarm on your phone, but if the battery dies, the alarm never goes off.
    *   **After:** Using a cloud alarm service. Even if your phone breaks, the service remembers to wake you up and will keep trying until you answer.
*   **👨‍💻 For the Senior Architect:**
    *   **Idempotency:** Crucial. If the payroll job runs twice due to a crash, ensure employees aren't paid double. Use unique job IDs.
    *   **Isolation:** Run heavy jobs on separate worker instances to prevent starving the web API threads.
    *   **Monitoring:** Integrate job failure alerts into the company's incident management system (e.g., PagerDuty).


### Options Comparison
| Tool | Best For | Persistence | Dashboard | Scalability |
|------|----------|-------------|-----------|-------------|
| **Hangfire** | Simple recurring jobs, retries | SQL/Redis | ✅ Built-in | Medium (with Redis) |
| **Quartz.NET** | Complex scheduling (cron) | JDBC/ADO.NET | ❌ (3rd party) | High |
| **IHostedService** | Lightweight, app-lifecycle jobs | None | ❌ | Low (single instance) |
| **Azure Functions** | Serverless, event-driven | Azure Storage | ✅ Azure Portal | ★★★★★ |
| **CAP** | Eventual consistency, outbox | SQL/Redis | ✅ | High |

### Hangfire Example
```csharp
// Startup.cs
services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(Configuration.GetConnectionString("Default")));

services.AddHangfireServer();

// Enqueue a job
BackgroundJob.Enqueue(() => EmailService.SendWelcomeEmail(userId));

// Recurring job
RecurringJob.AddOrUpdate<INotificationService>(
    "daily-report", 
    svc => svc.GenerateDailyReportAsync(), 
    Cron.Daily);

// Job with filter & retry
[AutomaticRetry(Attempts = 3)]
public class EmailService
{
    public void SendWelcomeEmail(string userId) { /* ... */ }
}
```

### IHostedService Pattern
```csharp
public class DataSyncService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DataSyncService> _logger;
    private Timer _timer;

    public DataSyncService(IServiceProvider services, ILogger<DataSyncService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(1));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        using var scope = _services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        try 
        {
            await dbContext.SyncExternalDataAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed");
        }
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return base.StopAsync(stoppingToken);
    }
}
```

### Azure Functions Timer Trigger
```csharp
public static class ReportGenerator
{
    [FunctionName("GenerateMonthlyReport")]
    public static async Task Run(
        [TimerTrigger("0 0 2 1 * *")] TimerInfo myTimer, // 2 AM, 1st of month
        [Blob("reports/{name}.pdf", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> output,
        ILogger log)
    {
        log.LogInformation($"Generating report at {myTimer.ScheduleStatus.Next}");
        var pdf = await ReportService.CreateMonthlyReportAsync();
        await output.AddAsync(pdf, new NameValueCollection { ["name"] = $"report-{DateTime.UtcNow:yyyy-MM}" });
    }
}
```

### Architecture: Reliable Job Processing
```
┌────────────────┐     ┌─────────────┐     ┌──────────────┐
│ HTTP Request   │     │ Job Queue   │     │ Worker       │
│ - Enqueue job  │────▶│ (Redis/SQL) │────▶│ (Hangfire/   │
│ - Return 202   │     │ - Persistence│    │  Functions)  │
└────────────────┘     └─────────────┘     └──────┬───────┘
                                                  │
                    ┌─────────────────────────────▼─────────────┐
                    │ Retry Policy │ Dead Letter │ Monitoring │
                    └──────────────────────────────────────────┘
```

### Best Practices
✅ Idempotency: Jobs should be safe to retry  
✅ Logging: Structured logs with JobId correlation  
✅ Timeouts: Prevent zombie jobs with CancellationToken  
✅ Separation: Different queues for critical vs. batch jobs  

### Common Pitfalls
❌ Capturing HttpContext in background jobs (null after request ends)  
❌ Not handling transient failures (network, DB locks)  
❌ Overloading a single queue with mixed-priority jobs  

### Interview Insights
> **Q**: "How do you ensure a background job doesn't run twice if a worker crashes mid-execution?"  
> **A**: Use **distributed locks** (Redis SETNX) or **lease-based processing** (Azure Functions singleton). For Hangfire, leverage its built-in job deduplication and transactional job storage.

Great question, Fareed — **this is exactly the kind of scaling problem enterprise systems worry about**, and Hangfire is only *one part* of the solution, not the whole solution.

Let me break this down in a clear, practical way with **real‑world enterprise scenarios**, and show you **how Hangfire fits in**, **where it doesn’t**, and **what you combine it with** in production systems dealing with millions of users.

***

# 🚀 **1. First: Can Hangfire handle millions of jobs?**

**Yes — Hangfire can scale to millions of jobs per day**, *but only when used correctly*.

Hangfire is used by companies processing:

*   Billing transactions
*   Background email sending
*   Notification pipelines
*   Report generation
*   Data processing
*   Media conversions (but not heavy ones!)

Hangfire is NOT meant to handle:

*   Heavy real-time media processing
*   Large file uploads
*   High-volume CPU-intensive tasks

⭐ Hangfire is great for **job orchestration and scheduling**, not for **storage or heavy compute**.

***

# 🏗️ **2. What happens if 100,000 users upload images or files at the same time?**

Let’s imagine:

*   1 million users total
*   100,000 users concurrently upload:
    *   Profile images
    *   CV PDFs
    *   Comments with attachments
    *   Videos

### ❌ If you try to store all files → Hangfire → SQL Server

You will kill:

*   Your Hangfire servers
*   Your SQL Server
*   Your disk throughput
*   Your CPU

Hangfire cannot compress, resize, convert, scan, index, and maintain these files at scale.

***

# 🧠 **3. So how do enterprise apps REALLY do it?**

Real‑world enterprise architecture uses **distributed components**:

    User Upload → API → Cloud Storage → Message Queue → Workers → DB Update

Hangfire is used as **one type of “worker”**, not the only one.

***

# 💼 **Real enterprise upload pipeline example (like Meta, LinkedIn, Upwork, Fiverr)**

### Step 1 — **User uploads file to object storage**

Files don’t go into the app server.

They go directly to:

*   Azure Blob Storage
*   AWS S3
*   Google Cloud Storage

This offloads **all heavy I/O from your application**.

***

### Step 2 — **API stores only metadata in DB**

The DB only stores:

*   File URL
*   File type
*   User ID
*   Processing state (Pending, Completed, Failed)

***

### Step 3 — **Queue triggers background workers**

Here is where Hangfire *may or may not* be used.

Typical enterprise queue systems:

| Queue Type            | Usage                         | Example                |
| --------------------- | ----------------------------- | ---------------------- |
| **Hangfire**          | Lightweight job orchestration | Notifications, emails  |
| **Azure Service Bus** | High-scale jobs               | File processing, AI    |
| **AWS SQS + Lambda**  | Auto-scaling compute          | Video processing       |
| **RabbitMQ**          | Real-time jobs                | Chat, small messages   |
| **Kafka**             | Very high throughput          | Activity logs, streams |

***

### Step 4 — **Workers process files**

This includes:

*   Image compression
*   Video transcoding
*   Virus scanning
*   PDF parsing
*   Text extraction
*   AI tagging

**These workers run on separate scalable servers**, not your main API.

Hangfire can be one worker type, but **it usually works WITH other workers**, not alone.

***

# 🧩 **How Hangfire fits in the system**

### Hangfire is ideal for:

*   Sending emails
*   Image thumbnail generation
*   Soft background tasks
*   Scheduling tasks
*   Database cleanup jobs
*   Reminders
*   Notifications

### Hangfire is NOT ideal for:

*   100k+ concurrent uploads
*   Heavy media processing
*   Machine learning pipelines
*   Real-time data streaming
*   CPU/GPU intensive tasks

***

# 🌍 **Real-world examples**

## 📸 Instagram / Facebook

When a user uploads an image:

*   Upload → (Cloud Storage)
*   Metadata → (Database)
*   Job queued in **Kafka**
*   Workers optimize → generate multiple resolutions
*   Result returned

No single Hangfire-like system can handle that load.

***

## 🎯 LinkedIn Job Applications

When users upload CV/resume:

*   Upload → Azure Blob Storage
*   A queue triggers a worker
*   Worker extracts: text, keywords, formatting
*   Data indexed into ElasticSearch

Hangfire could be used for:

*   Email notifications
*   Updating recruiter dashboard
*   Sending reminder emails

But **not for CV parsing** (too heavy).

***

## 🎬 YouTube or TikTok

Video upload:

*   Direct to storage
*   Message queue triggers FFmpeg workers
*   Workers run on GPU clusters
*   Output uploaded back to storage

Hangfire is way too small for this job.

***

# 🏢 **How Hangfire is used in enterprise .NET applications**

In large .NET systems, Hangfire is used for *supporting tasks*, not heavy workloads.

Example enterprise pattern:

    API → Save file to storage → Create Hangfire job for:
       - Thumbnail generation
       - Email notification
       - Audit logging
       - Sending a push notification

Heavy tasks are moved to:

*   Azure Functions
*   AWS Lambda
*   Kubernetes Jobs
*   Kafka consumers
*   SQS workers

***

# 🧨 Can Hangfire scale to 100k+ jobs?

Yes, **if properly scaled**:

*   10–20 worker servers
*   Redis instead of SQL
*   Partitioned queues
*   Auto-scaling workers
*   Dashboard monitoring

But **only for lightweight jobs**.

***

# 🏁 Final Summary

### ✔ Hangfire **can handle millions of jobs**, but only if:

*   Jobs are small
*   SQL load is offloaded
*   You use Redis
*   You scale worker servers

### ❌ Hangfire **should NOT handle**:

*   Heavy file processing
*   High-throughput uploads
*   Real-time tasks
*   Media transcoding

### 💡 Enterprise systems use:

*   Cloud storage
*   Distributed queues (Kafka/SQS/Service Bus)
*   Auto-scaled workers
*   Hangfire for orchestration and small tasks

***


**References**: 
- [Hangfire Docs](https://www.hangfire.io/documentation.html)
- [Microsoft: Background tasks](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services)
- [Azure Functions Triggers](https://learn.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings)

---

## CORS (Cross-Origin Resource Sharing)

### Overview
**Purpose**: Browser security mechanism that restricts cross-origin HTTP requests from web pages, preventing malicious sites from accessing APIs.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** JSONP (hacky), or disabling security in the browser, or proxying everything through the same domain.
*   **The Problem:**
    *   **Security Risks:** Allowing `*` (any origin) let malicious sites steal employee data.
    *   **Browser Blocks:** Modern browsers simply refused to connect frontend to backend.

### **The "After" State (CORS)**
*   **The Fix:** Explicit server headers telling the browser which websites are allowed to talk to the API.
*   **Employee Portal Scenario:**
    *   **Allowed:** `https://portal.company.com` (React App).
    *   **Blocked:** `https://malicious-site.com` (Trying to steal data).

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Disable CORS (Before)** | Everything works immediately | Extremely vulnerable to CSRF/XSS attacks |
| **Strict CORS (After)** | Secure, browser enforced | Configuration errors can break legitimate frontends |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Leaving your front door unlocked so anyone can walk in.
    *   **After:** Having a bouncer (Browser) who checks a guest list (CORS Policy). Only people on the list (trusted websites) can enter.
*   **👨‍💻 For the Senior Architect:**
    *   **Security:** CORS protects the **user**, not the server. Attackers using Postman ignore CORS. Always enforce Authentication/Authorization.
    *   **Performance:** Cache preflight (`OPTIONS`) requests using `SetPreflightMaxAge` to reduce latency.
    *   **Credentials:** Never combine `AllowAnyOrigin` with `AllowCredentials`.


**How it works**: 
- Browser sends preflight `OPTIONS` request for non-simple requests
- Server responds with `Access-Control-*` headers
- Browser allows/denies the actual request based on headers

### Simple vs. Preflight Requests
| Simple Request | Preflight Required |
|----------------|-------------------|
| `GET`, `HEAD`, `POST` | `PUT`, `DELETE`, `PATCH` |
| Content-Type: `application/x-www-form-urlencoded`, `multipart/form-data`, `text/plain` | Custom headers, `application/json` |
| No custom headers | `Authorization`, `X-Custom-Header` |

### ASP.NET Core Configuration
```csharp
// Program.cs - Minimal API
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("https://trusted-app.com", "https://admin.trusted-app.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials() // Required for cookies/auth headers
            .SetPreflightMaxAge(TimeSpan.FromHours(1))); // Cache preflight
});

var app = builder.Build();
app.UseCors("AllowSpecificOrigin"); // Order matters: before UseAuthorization

// Endpoint-specific CORS (override global)
app.MapPost("/api/orders", (Order order) => 
{
    // ...
})
.RequireCors("AllowSpecificOrigin");
```

### Middleware Pipeline Order (Critical!)
```
UseRouting() 
→ UseCors()          // ✅ Must be after routing, before auth
→ UseAuthentication()
→ UseAuthorization()
→ UseEndpoints()
```

### Real-World Scenarios
- **SPA + API**: React app on `localhost:3000` calling `api.example.com`
- **Microservices**: Service A (frontend) calling Service B (backend) from browser
- **Third-party integrations**: Widget embedded on partner sites

### Security Considerations
✅ **Never use `AllowAnyOrigin()` with `AllowCredentials()`** (browser blocks this)  
✅ Validate origins against allowlist, not just regex  
✅ Use `SetIsOriginAllowed(origin => whitelist.Contains(origin))` for dynamic validation  

### Common Pitfalls
❌ Forgetting preflight caching → excessive OPTIONS requests  
❌ Configuring CORS only on controllers (must be in middleware pipeline)  
❌ Testing CORS with Postman/curl (bypasses browser checks)  

### Interview Insights
> **Q**: "A developer says 'CORS is a server-side security feature'. Do you agree?"  
> **A**: Partially. CORS is a **browser-enforced** policy. Servers send headers to inform browsers, but malicious clients (curl, Postman) ignore them. CORS protects users, not APIs. Always authenticate/authorize server-side regardless of CORS.

**References**: 
- [MDN CORS Guide](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS)
- [ASP.NET Core CORS Docs](https://learn.microsoft.com/en-us/aspnet/core/security/cors)

---

## Redis

### Overview
**Purpose**: In-memory data structure store used for caching, pub/sub, rate limiting, distributed locks, and real-time analytics.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Static variables in code (`static Dictionary<>`) or hitting the SQL database for every single click.
*   **The Problem:**
    *   **Scale:** Static variables don't share data across multiple servers.
    *   **Performance:** Database choked when everyone logged in at 9 AM.

### **The "After" State (Redis)**
*   **The Fix:** A shared, super-fast in-memory database for caching and locking.
*   **Employee Portal Scenario:**
    *   **Cache:** Store `EmployeeProfile` data for 10 minutes to avoid DB hits.
    *   **Lock:** Prevent two managers from approving the same expense report simultaneously using a distributed lock.

### **Trade-offs**
| Strategy | Pros | Cons |
|----------|------|------|
| **DB Every Time (Before)** | Always fresh data | Slow, high DB load, expensive |
| **Redis Cache (After)** | Extremely fast, reduces DB load | Data can be stale (consistency), adds infrastructure cost |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Running to the library every time you need to check a fact in a book.
    *   **After:** Keeping a photocopy of the most important pages in your pocket. It's faster, but might be slightly outdated if the book changes.
*   **👨‍💻 For the Senior Architect:**
    *   **Consistency:** Define a **Cache Invalidation Strategy** (TTL vs. Write-Through). For salary data, use shorter TTLs.
    *   **Resilience:** Implement the **Circuit Breaker Pattern**. If Redis goes down, fail over to the DB gracefully rather than crashing the app.
    *   **Data Structures:** Use **Redis Hashes** for object storage and **Sets** for tracking unique active users.

### Core Data Structures & Use Cases
| Type | Commands | Use Case |
|------|----------|----------|
| **String** | `SET`, `GET`, `INCR` | Session cache, counters |
| **Hash** | `HSET`, `HGETALL` | User profiles, object caching |
| **List** | `LPUSH`, `BRPOP` | Task queues, recent items |
| **Set** | `SADD`, `SINTER` | Tags, unique visitors |
| **Sorted Set** | `ZADD`, `ZRANGE` | Leaderboards, priority queues |
| **Pub/Sub** | `PUBLISH`, `SUBSCRIBE` | Real-time notifications |

### C# Integration (StackExchange.Redis)
```csharp
// Connection multiplexer (singleton!)
private static readonly ConnectionMultiplexer _redis = 
    ConnectionMultiplexer.Connect("localhost:6379,password=...,ssl=True");

public class CacheService
{
    private readonly IDatabase _db = _redis.GetDatabase();

    // Cache-aside pattern
    public async Task<User> GetUserAsync(Guid id)
    {
        var cacheKey = $"user:{id}";
        var cached = await _db.StringGetAsync(cacheKey);
        
        if (cached.HasValue)
            return JsonSerializer.Deserialize<User>(cached);
        
        // Cache miss → load from DB
        var user = await _dbContext.Users.FindAsync(id);
        if (user != null)
        {
            // Set with expiration (slide/absolute)
            await _db.StringSetAsync(cacheKey, 
                JsonSerializer.Serialize(user), 
                TimeSpan.FromMinutes(30));
        }
        return user;
    }

    // Distributed lock
    public async Task<bool> ProcessWithLockAsync(string resource, Func<Task> action)
    {
        var lockKey = $"lock:{resource}";
        if (await _redis.GetDatabase().LockTakeAsync(lockKey, "worker1", TimeSpan.FromSeconds(30)))
        {
            try { await action(); return true; }
            finally { _redis.GetDatabase().LockRelease(lockKey, "worker1"); }
        }
        return false;
    }
}
```

### Pub/Sub Example
```csharp
// Publisher
await _db.PublishAsync("orders:created", JsonSerializer.Serialize(new OrderCreated { OrderId = id }));

// Subscriber (long-running service)
var subscriber = _redis.GetSubscriber();
await subscriber.SubscribeAsync("orders:created", (channel, message) =>
{
    var evt = JsonSerializer.Deserialize<OrderCreated>(message);
    // Handle event (e.g., update analytics)
});
```

### Architecture: Cache Strategies
```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ Application │     │   Redis     │     │   Database  │
│             │     │ (Cache)     │     │ (Source)    │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Cache-Aside │────▶│ 1. Check    │────▶│ 3. Load &   │
│ (Lazy Load) │     │    cache    │     │    cache    │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Write-Through│───▶│ 1. Update   │────▶│ 2. Update   │
│             │     │    cache    │     │    DB       │
├─────────────┤     ├─────────────┤     ├─────────────┤
│ Cache-Stampede│◀─│ Use locks/  │     │             │
│ Prevention │    │ probabilistic│     │             │
└─────────────┘   └─────────────┘     └─────────────┘
```

### Best Practices
✅ Use **connection multiplexer as singleton** (expensive to create)  
✅ Set **expiration** on all cache entries to prevent stale data  
✅ **Namespace keys**: `app:entity:id` to avoid collisions  
✅ Monitor **memory usage** and evictions (`INFO memory`)  

### Common Pitfalls
❌ Caching large objects without compression → memory pressure  
❌ Not handling cache failures gracefully (circuit breaker pattern)  
❌ Using Redis as primary database (no durability guarantees by default)  

### Interview Insights
> **Q**: "How do you handle cache invalidation in a distributed system?"  
> **A**: Combine strategies:  
> 1. Time-based expiration (TTL) for eventual consistency  
> 2. Explicit invalidation on writes (publish "invalidate:user:123" event)  
> 3. Cache-aside with versioned keys (`user:123:v2`) for critical data  
> Always design for cache failure: "Cache is a performance optimization, not a correctness guarantee."

**References**: 
- [Redis Docs](https://redis.io/documentation)
- [StackExchange.Redis](https://stackexchange.github.io/StackExchange.Redis/)
- [Redis Design Patterns](https://redis.io/topics/data-types-intro)

---

## JWT (JSON Web Tokens)

### Overview
**Purpose**: Compact, URL-safe token format for securely transmitting claims between parties, commonly used for stateless authentication.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Server-side sessions stored in memory or SQL.
*   **The Problem:**
    *   **Stickiness:** Users had to stay connected to the same server (hard in cloud environments).
    *   **Overhead:** Server had to look up session data for every request.

### **The "After" State (JWT)**
*   **The Fix:** Stateless tokens containing user claims (ID, Role) signed cryptographically.
*   **Employee Portal Scenario:**
    *   **Token:** Contains `EmployeeId`, `Department`, `Role`.
    *   **Usage:** API validates the signature without checking the database.

### **Trade-offs**
| Strategy | Pros | Cons |
|----------|------|------|
| **Server Sessions (Before)** | Easy to revoke instantly | Doesn't scale horizontally, memory intensive |
| **JWT (After)** | Stateless, scales infinitely | Hard to revoke before expiration, larger payload |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** The security guard memorizes your face. If you leave and come back, he might forget.
    *   **After:** You carry a stamped ticket. Any guard can read the stamp to know you're allowed in, without needing to call the main office.
*   **👨‍💻 For the Senior Architect:**
    *   **Payload:** Keep it small. Don't put sensitive data (PII) in JWTs as they are decoded easily.
    *   **Revocation:** Use short-lived Access Tokens (15 mins) + Long-lived Refresh Tokens. Store Refresh Tokens in DB to allow revocation upon employee termination.
    *   **Algorithm:** Prefer **RS256** (Asymmetric) over HS256 for microservices so services don't need to share secret keys.



### Token Structure
```
Header.Payload.Signature

Header:  { "alg": "HS256", "typ": "JWT" }
Payload: { "sub": "123", "name": "John", "admin": true, "exp": 1516239022 }
Signature: HMACSHA256(base64UrlEncode(header) + "." + base64UrlEncode(payload), secret)
```

### ASP.NET Core JWT Authentication
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        
        // Handle token expiration
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Generating a token (auth service)
public string GenerateToken(User user)
{
    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim("role", user.Role),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
    
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
    var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: creds);
    
    return new JwtSecurityTokenHandler().WriteToken(token);
}
```

### Refresh Token Pattern
```csharp
public class AuthResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; } // Stored hashed in DB
    public DateTime ExpiresAt { get; set; }
}

// Refresh endpoint
[HttpPost("refresh")]
public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
{
    var principal = GetPrincipalFromExpiredToken(request.AccessToken);
    var user = await _userService.GetUserAsync(principal.Identity.Name);
    
    // Validate refresh token (hashed comparison, expiration, revocation)
    if (!VerifyRefreshToken(request.RefreshToken, user.RefreshTokenHash))
        return Unauthorized();
    
    // Issue new tokens
    var newAccessToken = GenerateToken(user);
    var newRefreshToken = GenerateSecureRefreshToken();
    await _userService.UpdateRefreshTokenAsync(user.Id, newRefreshToken);
    
    return Ok(new AuthResponse { 
        AccessToken = newAccessToken, 
        RefreshToken = newRefreshToken,
        ExpiresAt = DateTime.UtcNow.AddMinutes(30)
    });
}
```

### Security Considerations
✅ **Short expiration** for access tokens (15-60 min)  
✅ **Store refresh tokens securely**: HttpOnly cookies or secure storage, hashed in DB  
✅ **Revoke tokens** on logout/password change (maintain blacklist or use reference tokens)  
✅ **Use RS256** (asymmetric) for microservices to avoid sharing secret keys  

### Common Pitfalls
❌ Storing sensitive data in JWT payload (it's base64-encoded, not encrypted)  
❌ Not validating `exp`, `nbf`, `iss`, `aud` claims  
❌ Using symmetric keys (HS256) across service boundaries  

### Interview Insights
> **Q**: "Why not just use cookies for authentication?"  
> **A**: Cookies require server-side session state (sticky sessions or shared cache). JWT enables **stateless authentication**, ideal for microservices and mobile apps. However, JWTs can't be easily revoked—combine with short TTL + refresh tokens or use a token introspection endpoint for critical systems.

**References**: 
- [JWT.io](https://jwt.io/)
- [RFC 7519](https://tools.ietf.org/html/rfc7519)
- [ASP.NET Core JWT Auth](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-auth)

---

## Unit Testing (xUnit) & Mocking (Moq)

### Overview
**Purpose**: Verify individual components in isolation, ensuring correctness and enabling safe refactoring.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Manual testing (clicking buttons), or no testing at all.
*   **The Problem:**
    *   **Regression:** Fixing one bug broke another feature unnoticed.
    *   **Fear:** Developers were scared to change old code.

### **The "After" State (Unit Testing)**
*   **The Fix:** Automated code tests that run every time changes are saved.
*   **Employee Portal Scenario:**
    *   **Test:** Verify `CalculateLeaveBalance()` returns correct days when an employee joins mid-year.
    *   **Mock:** Pretend the database exists without actually connecting to it.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Manual Testing (Before)** | Checks real user flow | Slow, expensive, prone to human error |
| **Automated Unit Tests (After)** | Fast, repeatable, catches regressions | High initial setup cost, requires maintenance |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Checking your math homework by guessing.
    *   **After:** Using a calculator to check every answer instantly. If you change a number, the calculator tells you if the total is wrong immediately.
*   **👨‍💻 For the Senior Architect:**
    *   **Coverage:** Aim for critical path coverage, not 100%. Test business logic, not getters/setters.
    *   **Isolation:** Use **Moq** to isolate external dependencies (Email, DB, APIs). Tests should run in milliseconds.
    *   **CI/CD:** Tests must be part of the build pipeline. If tests fail, deployment stops.

### xUnit Structure
```csharp
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repoMock;
    private readonly Mock<IPaymentGateway> _paymentMock;
    private readonly OrderService _sut; // System Under Test

    public OrderServiceTests()
    {
        _repoMock = new Mock<IOrderRepository>();
        _paymentMock = new Mock<IPaymentGateway>();
        _sut = new OrderService(_repoMock.Object, _paymentMock.Object);
    }

    [Fact]
    public async Task PlaceOrder_WithValidData_CallsPaymentAndSaves()
    {
        // Arrange
        var order = new Order { Id = Guid.NewGuid(), Total = 100m };
        _paymentMock.Setup(p => p.ChargeAsync(It.IsAny<PaymentRequest>()))
            .ReturnsAsync(new PaymentResult { Success = true });
        
        // Act
        var result = await _sut.PlaceOrderAsync(order);
        
        // Assert
        Assert.True(result.Success);
        _paymentMock.Verify(p => p.ChargeAsync(It.Is<PaymentRequest>(r => 
            r.Amount == order.Total)), Times.Once);
        _repoMock.Verify(r => r.AddAsync(order), Times.Once);
    }

    [Theory]
    [InlineData(-10)]
    [InlineData(0)]
    public async Task PlaceOrder_WithInvalidTotal_ThrowsArgumentException(decimal invalidTotal)
    {
        var order = new Order { Total = invalidTotal };
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.PlaceOrderAsync(order));
    }
}
```

### Advanced Mocking Techniques
```csharp
// Callback to capture arguments
PaymentRequest capturedRequest = null;
_paymentMock.Setup(p => p.ChargeAsync(It.IsAny<PaymentRequest>()))
    .Callback<PaymentRequest>(req => capturedRequest = req)
    .ReturnsAsync(new PaymentResult { Success = true });

// Mocking async IEnumerable
var items = new List<OrderItem> { new OrderItem { Price = 10m } }.AsAsyncEnumerable();
_repoMock.Setup(r => r.GetItemsAsync(It.IsAny<Guid>()))
    .Returns(items);

// Partial mocks (for legacy code)
var mockContext = new Mock<AppDbContext>();
mockContext.CallBase = true; // Use real methods unless mocked
mockContext.Setup(c => c.Orders.Find(It.IsAny<Guid>()))
    .Returns((Guid id) => new Order { Id = id, Status = "Mocked" });
```

### Testing ASP.NET Core Controllers
```csharp
[Fact]
public async Task GetOrder_ReturnsOkResult_WithOrder()
{
    // Arrange
    var order = new Order { Id = Guid.NewGuid(), Status = "Pending" };
    var mockMediator = new Mock<IMediator>();
    mockMediator.Setup(m => m.Send(It.IsAny<GetOrderQuery>(), default))
        .ReturnsAsync(order);
    
    var controller = new OrdersController(mockMediator.Object)
    {
        ControllerContext = new ControllerContext() // For HttpContext access
    };
    
    // Act
    var result = await controller.GetOrder(order.Id);
    
    // Assert
    var okResult = Assert.IsType<OkObjectResult>(result.Result);
    var returnedOrder = Assert.IsType<Order>(okResult.Value);
    Assert.Equal(order.Id, returnedOrder.Id);
}
```

### Best Practices
✅ **Test behavior, not implementation**: Mock interfaces, not concrete classes  
✅ **One assertion per test** (or logical group) for clear failures  
✅ **Use [Theory] + [InlineData]** for parameterized tests  
✅ **Isolate tests**: No shared state, use fresh mocks per test  

### Common Pitfalls
❌ Over-mocking: Testing mocks instead of real logic  
❌ Ignoring async: Forgetting `await` or `Task.FromResult`  
❌ Testing private methods: Refactor to make logic testable via public API  

### Interview Insights
> **Q**: "How do you test a method that depends on DateTime.Now?"  
> **A**: Inject `IDateTimeProvider` interface:  
> ```csharp
> public interface IDateTimeProvider { DateTime UtcNow { get; } }
> // In test: mock.Setup(p => p.UtcNow).Returns(new DateTime(2024, 1, 1));
> ```  
> Avoid `Microsoft.Extensions.TimeProvider` (new in .NET 8) for better testability.

**References**: 
- [xUnit Docs](https://xunit.net/)
- [Moq Quickstart](https://github.com/devlooped/moq/wiki/Quickstart)
- [Martin Fowler: Mocks Aren't Stubs](https://martinfowler.com/articles/mocksArentStubs.html)

---

## GraphQL

### Overview
**Purpose**: Query language for APIs allowing clients to request exactly the data they need, reducing over/under-fetching.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** REST APIs with fixed endpoints (e.g., `GET /employee/1`).
*   **The Problem:**
    *   **Over-fetching:** Getting all employee data (address, salary, history) when you only needed the name.
    *   **Under-fetching:** Needing to make 5 different API calls to build one dashboard screen.

### **The "After" State (GraphQL)**
*   **The Fix:** The client asks exactly for the fields it needs in one request.
*   **Employee Portal Scenario:**
    *   **Query:** Dashboard asks for `Employee.Name` and `LeaveBalance` only. Ignores `Salary` and `Address`.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **REST (Before)** | Simple, easy caching, standard | Rigid, over/under-fetching, multiple round trips |
| **GraphQL (After)** | Flexible, single round trip, strong typing | Complex caching, risk of heavy queries (DoS), harder to debug |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Ordering a fixed combo meal (Burger, Fries, Drink) even if you only wanted the Burger.
    *   **After:** Ordering exactly what you want (Just the Burger, no fries). You get exactly what you asked for, nothing wasted.
*   **👨‍💻 For the Senior Architect:**
    *   **N+1 Problem:** Use **DataLoaders** to batch database queries. Without this, fetching 100 employees might trigger 101 DB queries.
    *   **Security:** Implement **Query Complexity Analysis** to prevent clients from requesting deeply nested data that crashes the server.
    *   **Versioning:** GraphQL schemas evolve; avoid breaking changes by deprecating fields instead of removing them.

### Schema Design (Schema-First vs Code-First)
```csharp
// Code-First with GraphQL.NET
public class Query
{
    [GraphQLMetadata(Name = "user")]
    public async Task<User> GetUserAsync(Guid id, [FromServices] IUserService service)
        => await service.GetByIdAsync(id);
    
    [GraphQLMetadata(Name = "orders")]
    public async Task<IEnumerable<Order>> GetOrdersAsync(
        [FromServices] IOrderService service,
        string status = null,
        int limit = 10)
        => await service.GetFilteredAsync(status, limit);
}

public class Mutation
{
    [GraphQLMetadata]
    public async Task<Order> CreateOrderAsync(CreateOrderInput input, 
        [FromServices] IOrderService service)
        => await service.CreateAsync(input);
}

// Input types (avoid using entities directly)
public class CreateOrderInput
{
    public Guid UserId { get; set; }
    public List<OrderItemInput> Items { get; set; }
}
```

### ASP.NET Core Integration
```csharp
// Program.cs
builder.Services.AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering() // For pagination/sorting
    .AddProjections() // Map GraphQL fields to EF properties
    .RegisterService<IOrderService>()
    .AddApolloTracing(); // For performance monitoring

app.MapGraphQL("/graphql"); // Endpoint
```

### Query Example
```graphql
query GetUserWithOrders($userId: ID!) {
  user(id: $userId) {
    id
    name
    email
    orders(status: "SHIPPED", limit: 5) {
      id
      total
      items {
        product { name }
        quantity
      }
    }
  }
}
```

### N+1 Problem & DataLoaders
```csharp
// Problem: Fetching orders for multiple users causes N+1 queries
public class User
{
    public Guid Id { get; set; }
    // This would trigger a query per user if accessed naively
    public IEnumerable<Order> Orders { get; set; } 
}

// Solution: Batch DataLoader
public class OrderDataLoader : BatchDataLoader<Guid, Order>
{
    private readonly IOrderRepository _repo;
    
    public OrderDataLoader(IOrderRepository repo) => _repo = repo;
    
    protected override async Task<IReadOnlyDictionary<Guid, Order>> LoadBatchAsync(
        IReadOnlyList<Guid> keys, CancellationToken cancellationToken)
    {
        var orders = await _repo.GetByIdsAsync(keys, cancellationToken);
        return orders.ToDictionary(o => o.Id);
    }
}

// Usage in resolver
public async Task<IEnumerable<Order>> GetOrdersAsync(
    User user, 
    [Service] OrderDataLoader loader)
    => await loader.LoadAsync(user.Id); // Batches multiple user IDs
```

### Best Practices
✅ **Use input types** for mutations (never expose entities)  
✅ **Implement pagination** (`first`, `after`, `totalCount`)  
✅ **Authorize at field level** with `[Authorize]` attributes  
✅ **Limit query complexity** to prevent DoS (depth, cost analysis)  

### Common Pitfalls
❌ Exposing database entities directly in schema (over-fetching, security risks)  
❌ Not using DataLoaders → N+1 queries  
❌ Allowing arbitrary nested queries without depth limiting  

### Interview Insights
> **Q**: "When would you choose GraphQL over REST?"  
> **A**: GraphQL shines when:  
> - Clients have diverse data needs (mobile vs. web)  
> - Reducing network payload is critical (slow connections)  
> - Rapid frontend iteration requires flexible queries  
> Avoid GraphQL for: simple CRUD apps, file uploads, or when caching at HTTP level is essential.

**References**: 
- [GraphQL.NET](https://graphql-dotnet.github.io/)
- [Apollo Federation](https://www.apollographql.com/docs/federation/)
- [GraphQL Specification](https://spec.graphql.org/)

---

## FluentValidation

### Overview
**Purpose**: Fluent interface for building strongly-typed validation rules, separating validation logic from models.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** `if` statements inside Controllers or Business Logic (`if (email == null) throw...`).
*   **The Problem:**
    *   **Messy Code:** Business logic mixed with validation rules.
    *   **Repetition:** Same validation rules copied across different controllers.

### **The "After" State (FluentValidation)**
*   **The Fix:** Separate classes dedicated solely to validation rules.
*   **Employee Portal Scenario:**
    *   **Validator:** `CreateEmployeeValidator` ensures Email is valid, StartDate is not in the past, and Department exists.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Inline Ifs (Before)** | No extra libraries, simple for tiny apps | Spaghetti code, hard to reuse, hard to test |
| **FluentValidation (After)** | Clean separation, reusable, testable | Extra dependency, slight learning curve |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** A teacher checking your homework rules while trying to teach math. Distracting.
    *   **After:** A checklist robot that checks your homework format before the teacher even sees it. The teacher focuses on grading, the robot focuses on rules.
*   **👨‍💻 For the Senior Architect:**
    *   **Boundary:** Validation belongs at the **API Boundary** (Input), not the Domain Core (Business Rules).
    *   **Async:** Use async validation for database checks (e.g., "Is Email Unique?").
    *   **Pipeline:** Integrate with **MediatR** to run validation automatically before any command reaches the handler.

### Basic Validator
```csharp
public class CreateOrderValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderValidator(IUserService userService, IProductRepository productRepo)
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required")
            .MustAsync(async (id, ct) => await userService.ExistsAsync(id, ct))
            .WithMessage("User does not exist");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .Must(items => items.Sum(i => i.Quantity) > 0)
            .WithMessage("Order must have at least one item");
        
        RuleForEach(x => x.Items).ChildValidator(validator => 
            new OrderItemValidator(productRepo)); // Nested validation
        
        RuleFor(x => x.Total)
            .Equal(x => x.Items.Sum(i => i.Price * i.Quantity))
            .WithMessage("Total does not match items");
    }
}

public class OrderItemValidator : AbstractValidator<OrderItem>
{
    public OrderItemValidator(IProductRepository productRepo)
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).InclusiveBetween(1, 100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
```

### ASP.NET Core Integration
```csharp
// Program.cs
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();
builder.Services.AddMvc().AddFluentValidationAutoValidation(); // Auto-validates [FromBody] params

// Manual validation in MediatR pipeline
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        => _validators = validators;
    
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();
        
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Any()) 
            throw new ValidationException(failures); // Returns 400 in API
        
        return await next();
    }
}
```

### Advanced Rules
```csharp
// Conditional validation
RuleFor(x => x.ShippingAddress)
    .NotEmpty()
    .When(x => x.IsPhysicalProduct);

// Custom async rule
RuleFor(x => x.PromoCode)
    .MustAsync(async (code, ct) => 
    {
        if (string.IsNullOrEmpty(code)) return true;
        return await _promoService.IsValidAsync(code, ct);
    })
    .WithMessage("Invalid promo code");

// Dependent property validation
RuleFor(x => x.EndDate)
    .GreaterThan(x => x.StartDate)
    .WithMessage("End date must be after start date");
```

### Best Practices
✅ **Inject services** into validators for async database checks  
✅ **Use child validators** for complex nested objects  
✅ **Localize error messages** with `WithLocalizedName`  
✅ **Validate at the boundary** (API entry points), not in domain layer  

### Common Pitfalls
❌ Putting business logic in validators (validators should check *input*, not *rules*)  
❌ Overusing async validation (impacts performance)  
❌ Not testing validation rules independently  

### Interview Insights
> **Q**: "Where should validation live: controller, service, or domain?"  
> **A**: Layered approach:  
> - **FluentValidation**: Input sanitization & format checks (API boundary)  
> - **Domain entities**: Business invariants (e.g., `Order.Cancel()` checks status)  
> - **Database**: Unique constraints, foreign keys (last line of defense)  
> Never rely solely on client-side validation.

**References**: 
- [FluentValidation Docs](https://docs.fluentvalidation.net/)
- [ASP.NET Core Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)

---

## Serilog

### Overview
**Purpose**: Structured logging library that captures rich, queryable log events instead of plain text.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** `Console.WriteLine` or writing to plain text files.
*   **The Problem:**
    *   **Unsearchable:** Hard to find specific errors in massive text files.
    *   **No Context:** Logs said "Error occurred" but didn't say which employee or request caused it.

### **The "After" State (Serilog)**
*   **The Fix:** Structured logging (JSON) with context (EmployeeID, RequestID).
*   **Employee Portal Scenario:**
    *   **Log:** `"Event": "SalaryUpdated", "EmployeeId": 123, "User": "HR_Admin", "OldValue": 5000, "NewValue": 5500`

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Text Logs (Before)** | Simple, no setup | Hard to query, no structure, performance IO issues |
| **Structured Logs (After)** | Queryable, rich context, integrations | More storage space, requires log aggregator (Seq/ELK) |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Writing a diary in a secret code only you understand.
    *   **After:** Filling out a structured form (Date, Time, Event, Person). Later, you can easily search for "All events involving Person X".
*   **👨‍💻 For the Senior Architect:**
    *   **Observability:** Use **Correlation IDs** to trace a request across multiple microservices.
    *   **Performance:** Log asynchronously to prevent blocking the main thread.
    *   **Security:** **Redact PII** (Passwords, SSN) automatically using destructuring policies.


### Configuration (appsettings.json)
```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      { 
        "Name": "File",
        "Args": {
          "path": "logs/app-.log",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://localhost:5341" }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "OrderService"
    }
  }
}
```

### Program.cs Setup
```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithCorrelationIdHeader() // Custom enricher
    .CreateBootstrapLogger(); // For early startup logs

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

var app = builder.Build();

// Capture request logs
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("UserEmail", httpCtx.User?.Identity?.Name);
        diagCtx.Set("RequestId", Activity.Current?.Id ?? httpCtx.TraceIdentifier);
    };
});
```

### Structured Logging in Code
```csharp
public class OrderService
{
    private readonly ILogger<OrderService> _logger;
    
    public OrderService(ILogger<OrderService> logger) => _logger = logger;
    
    public async Task<Order> CreateOrderAsync(CreateOrderCommand cmd)
    {
        using var logContext = LogContext.PushProperty("OrderId", cmd.CorrelationId);
        
        _logger.LogInformation("Creating order for user {UserId} with {ItemCount} items", 
            cmd.UserId, cmd.Items.Count);
        
        try 
        {
            var order = await _repository.AddAsync(cmd.ToOrder());
            _logger.LogDebug("Order {OrderId} created successfully", order.Id);
            return order;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create order for user {UserId}", cmd.UserId);
            throw;
        }
    }
}
```

### Custom Enrichers & Sinks
```csharp
// Add correlation ID from header
public static class CorrelationIdEnricher
{
    public static LoggerConfiguration WithCorrelationIdHeader(this LoggerConfiguration lc)
        => lc.Enrich.With(new CorrelationIdEnricher());
}

public class CorrelationIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent evt, ILogEventPropertyFactory pf)
    {
        if (HttpContextAccessor.HttpContext?.Request.Headers.TryGetValue("X-Correlation-ID", out var id) == true)
        {
            evt.AddPropertyIfAbsent(pf.CreateProperty("CorrelationId", id.ToString()));
        }
    }
}

// Custom sink for auditing
public class AuditSink : ILogEventSink
{
    private readonly IAuditRepository _auditRepo;
    
    public void Emit(LogEvent evt)
    {
        if (evt.Level == LogEventLevel.Information && 
            evt.Properties.ContainsKey("Audit"))
        {
            _auditRepo.SaveAsync(evt.RenderMessage(), evt.Timestamp);
        }
    }
}
```

### Best Practices
✅ **Log structured data**, not concatenated strings (`"User {UserId}"` not `"User " + id`)  
✅ **Use appropriate log levels**: Debug (dev), Info (business events), Warning (recoverable), Error (failures)  
✅ **Avoid logging sensitive data** (PII, tokens) – use destructuring with `[NotLogged]`  
✅ **Sample high-volume logs** to reduce noise/cost  

### Common Pitfalls
❌ Logging exceptions without the exception object (`_logger.Error("Failed: " + ex.Message)`)  
❌ Over-enriching every log (performance impact)  
❌ Not configuring minimum levels per namespace (verbose framework logs)  

### Interview Insights
> **Q**: "How do you debug a production issue with distributed tracing?"  
> **A**: Combine Serilog with:  
> 1. **Correlation IDs**: Propagate `X-Correlation-ID` across services  
> 2. **OpenTelemetry**: Export traces to Jaeger/Zipkin  
> 3. **Seq/ELK**: Query logs by `CorrelationId` to see full request flow  
> Always log at the entry/exit of critical operations with contextual properties.

**References**: 
- [Serilog Wiki](https://github.com/serilog/serilog/wiki)
- [Structured Logging with Serilog](https://stackify.com/structured-logging-serilog/)
- [ASP.NET Core Logging](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

---

## AutoMapper

### Overview
**Purpose**: Convention-based object-object mapper to reduce boilerplate code for DTO/entity conversions.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Manual mapping (`dto.Name = entity.Name`).
*   **The Problem:**
    *   **Boilerplate:** Hundreds of lines of repetitive code.
    *   **Errors:** Easy to forget a field when adding new properties.

### **The "After" State (AutoMapper)**
*   **The Fix:** Convention-based mapping configuration.
*   **Employee Portal Scenario:**
    *   **Map:** `EmployeeEntity` → `EmployeeDto`. Automatically matches properties with the same name.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Manual Mapping (Before)** | Explicit, full control, fast | Repetitive, prone to human error, verbose |
| **AutoMapper (After)** | Less code, maintainable, clear intent | Performance overhead (reflection), "Magic" behavior |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Copying notes from the blackboard to your notebook by hand.
    *   **After:** Using a photocopier. It does the work instantly, but you need to make sure the paper is aligned correctly first.
*   **👨‍💻 For the Senior Architect:**
    *   **Performance:** In high-throughput APIs, measure the overhead. Consider **Mapster** or manual mapping for critical paths.
    *   **Projection:** Use `ProjectTo<T>` for EF Core to translate mapping directly to SQL, avoiding loading full entities into memory.
    *   **Explicitness:** Don't map everything. Configure specific profiles to avoid accidental data leakage (e.g., mapping PasswordHash to DTO).

---

### Basic Configuration
```csharp
// Profile definition
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"))
            .ForMember(d => d.Roles, opt => opt.MapFrom(s => s.UserRoles.Select(ur => ur.Role.Name)));
        
        CreateMap<CreateUserCommand, User>()
            .ForMember(s => s.PasswordHash, opt => opt.Ignore()) // Set manually
            .ForMember(s => s.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        
        // Reverse map for updates
        CreateMap<UpdateUserCommand, User>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}

// Registration
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

### Usage in Services
```csharp
public class UserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _repo;
    
    public async Task<UserDto> GetUserAsync(Guid id)
    {
        var user = await _repo.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }
    
    public async Task UpdateUserAsync(Guid id, UpdateUserCommand cmd)
    {
        var user = await _repo.GetByIdAsync(id) ?? throw new NotFoundException();
        _mapper.Map(cmd, user); // Updates only non-null properties
        await _repo.UpdateAsync(user);
    }
}
```

### Advanced Scenarios
```csharp
// Custom value resolver
public class FullNameResolver : IValueResolver<User, UserDto, string>
{
    public string Resolve(User src, UserDto dest, string member, ResolutionContext ctx)
        => $"{src.FirstName} {src.LastName}".Trim();
}

// Conditional mapping
CreateMap<Order, OrderDto>()
    .ForMember(d => d.StatusText, opt => opt.MapFrom((src, dest, member, ctx) =>
        src.Status switch
        {
            OrderStatus.Pending => "Awaiting Payment",
            OrderStatus.Shipped => "On the Way",
            _ => src.Status.ToString()
        }));

// Projection to IQueryable (for EF Core)
var query = _context.Users.ProjectTo<UserDto>(_mapper.ConfigurationProvider);
// Translates to efficient SQL, no client-side evaluation
```

### Performance Considerations
✅ **Precompile mappings** in startup (AutoMapper does this by default)  
✅ **Use ProjectTo<T>** for EF Core queries to avoid loading full entities  
✅ **Avoid mapping large graphs**; map only needed fields  

### When NOT to Use AutoMapper
❌ Simple 1:1 mappings (manual mapping is clearer)  
❌ Complex business logic in mapping (belongs in service layer)  
❌ Performance-critical paths (measure before optimizing)  

### Interview Insights
> **Q**: "How do you handle breaking changes in DTOs without breaking clients?"  
> **A**:  
> 1. **Version APIs**: `/api/v1/users`, `/api/v2/users`  
> 2. **Use [Obsolete] attributes** with migration path  
> 3. **Map multiple versions**:  
> ```csharp
> CreateMap<User, UserDtoV1>();
> CreateMap<User, UserDtoV2>();
> // Controller selects based on Accept header
> ```  
> AutoMapper helps maintain multiple mappings without duplicating entity logic.

**References**: 
- [AutoMapper Docs](https://docs.automapper.org/)
- [Mapping vs Manual Code](https://jimmybogard.com/should-you-use-automapper/)

---

## Middleware & Custom Middleware

### Overview
**Purpose**: Components assembled into an application pipeline to handle requests and responses.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Base Controllers or Global Filters.
*   **The Problem:**
    *   **Coupling:** Every controller had to inherit from a specific base class.
    *   **Rigid:** Hard to apply logic to specific routes only.

### **The "After" State (Middleware)**
*   **The Fix:** Components in the request pipeline that run before/after the controller.
*   **Employee Portal Scenario:**
    *   **Middleware:** `ExceptionHandlingMiddleware` catches errors globally. `TenantMiddleware` identifies which company is accessing the portal.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Base Controllers (Before)** | Easy to share logic | Inheritance hell, limited to MVC |
| **Middleware (After)** | Decoupled, works for all requests | Order sensitive, can impact performance if heavy |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Every student having to clean their own desk before leaving class.
    *   **After:** A cleaning crew (Middleware) that walks through the hallway after everyone leaves. The students just focus on learning.
*   **👨‍💻 For the Senior Architect:**
    *   **Pipeline Order:** Critical. Auth must happen before Authorization. CORS must happen before Auth.
    *   **Short-circuiting:** Use middleware to reject requests early (e.g., Maintenance Mode) to save resources.
    *   **Scope:** Be careful injecting Scoped services into Singleton Middleware (use `IServiceProvider` within `Invoke`).

### Built-in Middleware Order (Critical!)
```csharp
app.UseExceptionHandler();        // Error handling (early)
app.UseHsts();                    // Security headers
app.UseHttpsRedirection();
app.UseStaticFiles();             // wwwroot files
app.UseRouting();                 // Endpoint routing
app.UseCors();                    // CORS (after routing)
app.UseAuthentication();          // AuthN
app.UseAuthorization();           // AuthZ
app.UseRateLimiter();             // Throttling
app.MapControllers();             // Endpoints
```

### Custom Middleware Example
```csharp
// Middleware class
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        // Before request
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;
        
        try
        {
            await _next(context);
            stopwatch.Stop();
            
            // After successful request
            _logger.LogInformation("{Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                context.Response.StatusCode);
            
            // Copy response back
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "{Method} {Path} failed after {ElapsedMs}ms",
                context.Request.Method, context.Request.Path, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}

// Extension method for clean registration
public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder builder)
        => builder.UseMiddleware<RequestTimingMiddleware>();
}

// Usage in Program.cs
app.UseRequestTiming();
```

### Short-Circuiting Middleware
```csharp
// Maintenance mode middleware
public class MaintenanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _config;
    
    public async Task InvokeAsync(HttpContext context)
    {
        if (_config.GetValue<bool>("MaintenanceMode") && 
            !context.Request.Path.StartsWithSegments("/admin"))
        {
            context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await context.Response.WriteAsJsonAsync(new { message = "System under maintenance" });
            return; // Short-circuit: don't call _next
        }
        await _next(context);
    }
}
```

### Best Practices
✅ **Keep middleware focused**: One responsibility per component  
✅ **Handle exceptions** to avoid breaking the pipeline  
✅ **Avoid async void**; always return `Task`  
✅ **Use IMiddleware** for scoped services (vs constructor injection for singletons)  

### Common Pitfalls
❌ Modifying `HttpContext.Items` without thread-safety (use `AsyncLocal`)  
❌ Reading request body multiple times (enable buffering: `context.Request.EnableBuffering()`)  
❌ Placing middleware in wrong order (e.g., CORS after authorization)  

### Interview Insights
> **Q**: "How would you implement API versioning via middleware?"  
> **A**:  
> ```csharp
> public class ApiVersionMiddleware
> {
>     public async Task InvokeAsync(HttpContext context)
>     {
>         var version = context.Request.Headers["api-version"].FirstOrDefault() ?? "1.0";
>         context.Items["ApiVersion"] = Version.Parse(version);
>         
>         await _next(context);
>         
>         // Add version to response header
>         context.Response.Headers["api-version"] = version;
>     }
> }
> ```  
> Then in controllers:  
> ```csharp
> [MapToApiVersion("1.0")]
> public class UsersController : ControllerBase { ... }
> ```  
> Combine with endpoint routing for clean versioned APIs.

**References**: 
- [ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Middleware Order Guide](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/#middleware-order)

---

## Request/Response Logging

### Overview
**Purpose**: Capture HTTP traffic for debugging, auditing, and monitoring without modifying business logic.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Debugging in Production (bad!) or no logging of inputs/outputs.
*   **The Problem:**
    *   **Blind Spots:** Couldn't reproduce bugs because you didn't know what data was sent.
    *   **Audit Gaps:** No record of who changed sensitive employee data.

### **The "After" State (Logging Middleware)**
*   **The Fix:** Automatically log every HTTP request and response payload.
*   **Employee Portal Scenario:**
    *   **Audit:** Log every `PUT /salary` request with the old and new values for compliance.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **No Logging (Before)** | Fast, private | Impossible to debug, no audit trail |
| **Full Payload Logging (After)** | Complete visibility, audit ready | Performance cost, privacy risks (PII leakage) |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Sending a letter without keeping a copy. If it gets lost, you don't know what you wrote.
    *   **After:** Keeping a carbon copy of every letter sent and received. You can always check what happened.
*   **👨‍💻 For the Senior Architect:**
    *   **Privacy:** **Must** implement sanitization (redact passwords, tokens, SSN) before logging.
    *   **Performance:** Log asynchronously. Don't log large payloads (files) synchronously.
    *   **Sampling:** In high traffic, log only 1% of successful requests but 100% of errors.

---

### Techniques Comparison
| Approach | Pros | Cons |
|----------|------|------|
| **Middleware** | Centralized, low overhead | Can't access endpoint-specific context |
| **Action Filters** | Endpoint-aware, MVC-specific | Only works for controllers |
| **DelegatingHandler** | For HTTP client logging | Only outgoing requests |
| **Serilog RequestLogging** | Structured, configurable | Requires Serilog setup |

### Serilog Request Logging (Recommended)
```csharp
// In Program.cs
app.UseSerilogRequestLogging(options =>
{
    // Customize message template
    options.MessageTemplate = 
        "HTTP {RequestMethod} {RequestPath} => {StatusCode} ({ElapsedMilliseconds}ms)";
    
    // Add custom properties
    options.EnrichDiagnosticContext = (diagCtx, httpCtx) =>
    {
        diagCtx.Set("UserEmail", httpCtx.User?.Identity?.Name);
        diagCtx.Set("RequestContentLength", httpCtx.Request.ContentLength);
        
        // Log request headers (careful with sensitive data!)
        if (httpCtx.Request.Headers.TryGetValue("X-Client-Version", out var version))
            diagCtx.Set("ClientVersion", version.ToString());
    };
    
    // Conditional logging (e.g., only errors)
    options.GetLevel = (httpCtx, elapsed, ex) => 
        ex != null ? LogEventLevel.Error : 
        httpCtx.Response.StatusCode >= 500 ? LogEventLevel.Warning : 
        LogEventLevel.Information;
});
```

### Manual Middleware for Full Payload Logging (Use Sparingly!)
```csharp
public class PayloadLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PayloadLoggingMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Enable request body buffering
        context.Request.EnableBuffering();
        
        // Read request body
        var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0; // Reset for downstream
        
        // Capture response body
        var originalBody = context.Response.Body;
        using var responseBuffer = new MemoryStream();
        context.Response.Body = responseBuffer;
        
        try
        {
            await _next(context);
            
            // Read response
            responseBuffer.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBuffer).ReadToEndAsync();
            responseBuffer.Seek(0, SeekOrigin.Begin);
            await responseBuffer.CopyToAsync(originalBody);
            
            // Log (redact sensitive fields!)
            _logger.LogDebug("Request: {RequestBody}\nResponse: {ResponseBody}",
                Sanitize(requestBody), Sanitize(responseBody));
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }
    
    private string Sanitize(string json)
    {
        // Use regex or JSON parser to redact password, token, etc.
        return Regex.Replace(json, @"(""password""\s*:\s*"")[^""]+", "$1***");
    }
}
```

### Best Practices
✅ **Log at appropriate level**: Debug for payloads, Info for metadata  
✅ **Redact sensitive data**: Passwords, tokens, PII (use configuration-driven rules)  
✅ **Sample high-traffic endpoints** to avoid log explosion  
✅ **Include correlation IDs** for distributed tracing  

### Common Pitfalls
❌ Logging full request/response bodies in production (performance, privacy)  
❌ Not resetting stream positions → downstream middleware fails  
❌ Synchronous logging in async pipeline (use `await` for I/O)  

### Interview Insights
> **Q**: "How do you log requests without impacting performance?"  
> **A**:  
> 1. **Async logging**: Serilog sinks write asynchronously  
> 2. **Conditional logging**: Only log errors or sample 1% of requests  
> 3. **Buffer size limits**: Skip logging bodies > 1MB  
> 4. **Offload to dedicated service**: Ship logs to Seq/ELK via TCP, not disk I/O  
> Always measure: use `ILogger.BeginScope` with `RequestTimingMiddleware` to identify bottlenecks.

**References**: 
- [Serilog RequestLogging](https://github.com/serilog/serilog-aspnetcore#request-logging)
- [ASP.NET Core Logging Best Practices](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/)

---

## CQRS

### Overview
**Purpose**: Separate read and write operations to optimize scalability, performance, and complexity management.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** One model (Entity) used for both saving data and reading data.
*   **The Problem:**
    *   **Complexity:** The `Employee` class became huge with logic for saving, reading, reporting, and validation.
    *   **Performance:** Read queries loaded heavy entities when only a summary was needed.

### **The "After" State (CQRS)**
*   **The Fix:** Separate models for writing (Commands) and reading (Queries).
*   **Employee Portal Scenario:**
    *   **Write:** `UpdateEmployeeCommand` (Validates rules, saves to DB).
    *   **Read:** `GetEmployeeSummaryQuery` (Returns simple DTO, maybe from a cache).

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **CRUD Model (Before)** | Simple, consistent | Doesn't scale, complex domain logic gets messy |
| **CQRS (After)** | Scalable, optimized models | Complexity, eventual consistency, more code |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Using the same form to apply for a job and to check your pay stub. It's confusing and has too many fields.
    *   **After:** One form for applying (Write) and a different screen for viewing pay (Read). Each is designed perfectly for its specific job.
*   **👨‍💻 For the Senior Architect:**
    *   **Consistency:** Accept **Eventual Consistency** on the read side. The dashboard might show old data for a few seconds after an update.
    *   **Optimization:** Read models can be denormalized (store Employee Name in the Order table) to avoid Joins.
    *   **When to use:** Don't use for simple CRUD. Use when read/write loads differ significantly (e.g., 1000 reads vs 1 write).

---

### Core Principles
- **Commands**: Change state (CreateOrder), return void/ID, handled by one handler
- **Queries**: Read state (GetOrder), return DTOs, can have multiple handlers
- **Separate models**: Write model (domain entities) ≠ Read model (denormalized DTOs)

### Basic Implementation (Without Event Sourcing)
```csharp
// Commands
public class CreateOrderCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderRepository _repo;
    private readonly IEventPublisher _events;
    
    public async Task<Guid> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        var order = Order.Create(cmd.UserId, cmd.Items); // Domain logic
        await _repo.AddAsync(order, ct);
        await _events.PublishAsync(order.GetDomainEvents(), ct);
        return order.Id;
    }
}

// Queries
public class GetOrderQuery : IRequest<OrderDto>
{
    public Guid OrderId { get; set; }
}

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderDto>
{
    private readonly IReadDbContext _readDb; // Optimized read DB
    
    public async Task<OrderDto> Handle(GetOrderQuery query, CancellationToken ct)
    {
        // Direct projection to DTO, no domain logic
        return await _readDb.Orders
            .Where(o => o.Id == query.OrderId)
            .Select(o => new OrderDto {
                Id = o.Id,
                Total = o.Total,
                Status = o.Status.ToString(),
                // Denormalized fields
                CustomerEmail = o.User.Email
            })
            .FirstOrDefaultAsync(ct);
    }
}
```

### Advanced: Separate Read/Write Databases
```
┌─────────────────┐     ┌─────────────┐     ┌─────────────────┐
│ Write Side      │     │  Event Bus  │     │ Read Side       │
│ - Domain Model  │────▶│ (RabbitMQ)  │────▶│ - Denormalized  │
│ - EF Core (SQL) │     │ - Outbox    │     │   Views (NoSQL) │
│ - Commands      │     │             │     │ - Queries       │
└─────────────────┘     └─────────────┘     └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │ Projection      │
                    │ Services        │
                    │ - Update read   │
                    │   models on     │
                    │   domain events │
                    └─────────────────┘
```

### When to Use CQRS
✅ Complex domains with different read/write patterns  
✅ High read scalability needs (cache read models)  
✅ Audit/compliance requirements (event sourcing)  
✅ Team separation: write team vs. read team  

### When to Avoid
❌ Simple CRUD applications (YAGNI)  
❌ Strong consistency requirements across read/write  
❌ Small teams (adds complexity)  

### Best Practices
✅ Start with **logical separation** (same DB, different models) before physical separation  
✅ Use **MediatR** for in-process command/query dispatch  
✅ **Project domain events** to read models asynchronously  
✅ **Version read models** to handle schema changes  

### Common Pitfalls
❌ Over-engineering simple apps with CQRS  
❌ Ignoring eventual consistency in UI (show "updating..." states)  
❌ Duplicating validation logic between command handlers and read models  

### Interview Insights
> **Q**: "How do you handle a query that needs data from multiple aggregates?"  
> **A**: In CQRS:  
> 1. **Denormalize** the read model to include needed data (e.g., store customer name in order read model)  
> 2. **Use a read-side projector** that listens to `CustomerNameChanged` and `OrderCreated` events to update the denormalized view  
> 3. **Accept eventual consistency**: UI shows last known state, with optimistic updates  
> Never join aggregates in the write model—this violates bounded context boundaries.

**References**: 
- [Martin Fowler: CQRS](https://martinfowler.com/bliki/CQRS.html)
- [Greg Young: CQRS Documents](https://www.cqrs.nu/)
- [eShopOnContainers CQRS Sample](https://github.com/dotnet/eShop)

---

## Repository Pattern & Unit of Work

### Overview
**Purpose**: Abstract data access logic to decouple business logic from persistence technology and enable testability.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** SQL queries written directly inside Controllers or Services.
*   **The Problem:**
    *   **Coupling:** Changing from SQL Server to PostgreSQL required rewriting every controller.
    *   **Testing:** Couldn't test logic without a real database.

### **The "After" State (Repository Pattern)**
*   **The Fix:** An abstraction layer between business logic and database.
*   **Employee Portal Scenario:**
    *   **Interface:** `IEmployeeRepository.GetById()`.
    *   **Implementation:** `SqlEmployeeRepository` (uses EF Core).

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Direct SQL (Before)** | Fast, full control | Tightly coupled, hard to test, hard to swap DB |
| **Repository (After)** | Testable, decoupled, clean | Can be over-engineering with EF Core (which is already a repo) |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Going into the library stacks yourself to find books.
    *   **After:** Asking the Librarian (Repository). You don't need to know how the books are organized; you just ask for what you need.
*   **👨‍💻 For the Senior Architect:**
    *   **Leaky Abstraction:** Don't expose `IQueryable` from the repository. It leaks EF Core specifics to the business layer.
    *   **EF Core Context:** EF Core `DbContext` is already a Unit of Work. Wrapping it in another `IUnitOfWork` is often redundant unless coordinating multiple DBs.
    *   **Specific vs. Generic:** Prefer specific repositories (`IEmployeeRepo`) over generic (`IRepository<T>`) to express domain language.

---

### Generic Repository (Use Sparingly)
```csharp
public interface IRepository<T> where T : AggregateRoot
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}

public class EfRepository<T> : IRepository<T> where T : AggregateRoot
{
    protected readonly AppDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;
    
    public EfRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }
    
    public async Task<T> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _dbSet.FindAsync(new object[] { id }, ct);
    
    public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec, CancellationToken ct = default)
        => await ApplySpecification(spec).ToListAsync(ct);
    
    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        => SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
}
```

### Specification Pattern (for complex queries)
```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    // ... ordering, paging, etc.
}

public class OrdersByUserSpec : ISpecification<Order>
{
    public Expression<Func<Order, bool>> Criteria 
        => o => o.UserId == _userId && o.Status == OrderStatus.Pending;
    
    public List<Expression<Func<Order, object>>> Includes 
        => new() { o => o.Items, o => o.User };
    
    private readonly Guid _userId;
    public OrdersByUserSpec(Guid userId) => _userId = userId;
}

// Usage
var spec = new OrdersByUserSpec(userId);
var orders = await _repo.ListAsync(spec, ct);
```

### Unit of Work (Often Redundant with EF Core)
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Order> Orders { get; }
    IRepository<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public class EfUnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IRepository<Order> _orders;
    
    public IRepository<Order> Orders 
        => _orders ??= new EfRepository<Order>(_context);
    
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
    
    public void Dispose() => _context.Dispose();
}
```

### Modern Approach: EF Core as Implementation Detail
```csharp
// Prefer application-specific interfaces over generic repos
public interface IOrderRepository
{
    Task<Order> GetByIdWithItemsAsync(Guid id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task<bool> IsProductAvailableAsync(Guid productId, int quantity, CancellationToken ct);
}

public class SqlOrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    
    public async Task<Order> GetByIdWithItemsAsync(Guid id, CancellationToken ct)
        => await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id, ct);
    
    // Custom query logic here
}
```

### Best Practices
✅ **Abstract only when beneficial**: Don't wrap EF Core just for the sake of it  
✅ **Use specifications** for complex, reusable queries  
✅ **Return `IReadOnlyList<T>`** to prevent unintended modifications  
✅ **Keep repositories focused** on one aggregate root  

### Common Pitfalls
❌ Leaking `IQueryable<T>` from repository (breaks encapsulation)  
❌ Using generic repository for all entities (leads to anemic domain)  
❌ Implementing UoW when EF Core's `SaveChanges` already provides it  

### Interview Insights
> **Q**: "Is the repository pattern still relevant with EF Core?"  
> **A**: Yes, but evolved:  
> - **Use case 1**: Swap persistence tech (rare in .NET shops)  
> - **Use case 2**: **Testability** – mock `IOrderRepository` in unit tests  
> - **Use case 3**: **Encapsulate complex queries** behind intention-revealing interfaces  
> Avoid generic repositories; prefer application-specific interfaces that express domain language. EF Core's `DbContext` is already a unit of work—wrap it only if you need to coordinate multiple contexts.

**References**: 
- [Martin Fowler: Repository](https://martinfowler.com/eaaCatalog/repository.html)
- [EF Core Best Practices](https://learn.microsoft.com/en-us/ef/core/miscellaneous/async)
- [Specification Pattern](https://deviq.com/design-patterns/specification-pattern)

---

## MediatR, Handlers, Request/Response Models

### Overview
**Purpose**: Implement the mediator pattern to decouple components, reduce coupling, and enable cross-cutting concerns via pipelines.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** Controllers calling Services directly (`_service.DoWork()`).
*   **The Problem:**
    *   **Fat Controllers:** Controllers became bloated with logic.
    *   **Coupling:** Controller knew exactly which service class to call.

### **The "After" State (MediatR)**
*   **The Fix:** Controller sends a "Message" (Request). A separate "Handler" picks it up.
*   **Employee Portal Scenario:**
    *   **Request:** `PromoteEmployeeCommand`.
    *   **Handler:** `PromoteEmployeeHandler` (Contains the logic).
    *   **Benefit:** Easy to add logging or validation pipelines without changing the controller.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Direct Service Call (Before)** | Simple, easy to trace | Tight coupling, cross-cutting concerns repeated |
| **MediatR (After)** | Decoupled, clean controllers, pipelines | Indirection (harder to debug), performance overhead |

### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Walking over to your friend's desk to tell them to do something.
    *   **After:** Putting a note in a mailbox (Mediator). A worker picks up the note and delivers it. You don't need to know where the friend sits.
*   **👨‍💻 For the Senior Architect:**
    *   **Vertical Slice Architecture:** Group code by Feature (e.g., "Promotion") rather than Layer (Controllers, Services, Repos). MediatR enables this.
    *   **Pipelines:** Use behaviors for cross-cutting concerns (Transactions, Logging, Validation) centrally.
    *   **Scope:** MediatR is **In-Process**. It does not work across microservices (use MassTransit/NServiceBus for that).

---

### Core Concepts
- **Requests**: Commands (`IRequest<Unit>`) or Queries (`IRequest<TResponse>`)
- **Handlers**: Single class implements `IRequestHandler<TRequest, TResponse>`
- **Pipelines**: Behaviors for logging, validation, caching, transactions

### Basic Setup
```csharp
// Request/Response models
public class CreateOrderCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public List<OrderItemDto> Items { get; set; }
}

public class GetOrderQuery : IRequest<OrderDto>
{
    public Guid OrderId { get; set; }
}

// Handler
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IOrderService _service;
    
    public CreateOrderCommandHandler(IOrderService service) 
        => _service = service;
    
    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
        => await _service.CreateOrderAsync(request, ct);
}

// Registration
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssemblyContaining<Program>());
```

### Pipeline Behaviors (Cross-Cutting Concerns)
```csharp
// Validation behavior (integrates with FluentValidation)
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        => _validators = validators;
    
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();
        
        var context = new ValidationContext<TRequest>(request);
        var failures = _validators.Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        if (failures.Any()) throw new ValidationException(failures);
        return await next();
    }
}

// Logging behavior
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
        => _logger = logger;
    
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;
        _logger.LogInformation("Handling {RequestName} {@Request}", requestName, request);
        
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();
        
        _logger.LogInformation("Handled {RequestName} in {ElapsedMs}ms", 
            requestName, stopwatch.ElapsedMilliseconds);
        
        return response;
    }
}

// Register behaviors (order matters!)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

### Advanced: Transactional Behavior
```csharp
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly AppDbContext _dbContext;
    
    public TransactionBehavior(AppDbContext dbContext) => _dbContext = dbContext;
    
    public async Task<TResponse> Handle(TRequest request, 
        RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);
            try
            {
                var response = await next();
                await _dbContext.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }
}
```

### Best Practices
✅ **One handler per request type** (enforces single responsibility)  
✅ **Use pipeline behaviors** for cross-cutting concerns (DRY)  
✅ **Keep requests/responses anemic** (no behavior, just data)  
✅ **Avoid chatty interfaces**: Batch requests when possible  

### Common Pitfalls
❌ Overusing MediatR for simple CRUD (adds indirection)  
❌ Capturing `HttpContext` in handlers (breaks testability)  
❌ Not handling cancellation tokens properly in async pipelines  

### Interview Insights
> **Q**: "How do you handle distributed transactions across multiple services with MediatR?"  
> **A**: MediatR is in-process. For distributed systems:  
> 1. Use **Saga pattern** with state machine (e.g., MassTransit)  
> 2. **Outbox pattern**: Save events to DB in same transaction as business logic, then publish  
> 3. **Idempotency keys**: Prevent duplicate processing in handlers  
> MediatR excels at in-process decoupling; combine with message brokers for cross-service workflows.

**References**: 
- [MediatR Docs](https://github.com/jbogard/MediatR)
- [Pipeline Behaviors Guide](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/)
- [eShopOnContainers MediatR Usage](https://github.com/dotnet/eShop)

---

## Entity Framework Core & DbContext

### Overview
**Purpose**: Object-relational mapper (ORM) enabling .NET developers to work with databases using .NET objects.

### **The "Before" State (Legacy Implementation)**
*   **How it was done:** ADO.NET, Dapper, writing raw SQL strings.
*   **The Problem:**
    *   **Productivity:** Writing SQL for every single table operation was slow.
    *   **Security:** High risk of SQL Injection if strings were concatenated poorly.

### **The "After" State (EF Core)**
*   **The Fix:** Object-Relational Mapper. Work with C# objects, EF translates to SQL.
*   **Employee Portal Scenario:**
    *   **Code:** `context.Employees.Where(e => e.IsActive)`.
    *   **SQL:** EF generates `SELECT * FROM Employees WHERE IsActive = 1`.

### **Trade-offs**
| Approach | Pros | Cons |
|----------|------|------|
| **Raw SQL (Before)** | Maximum performance, full control | Verbose, security risks, hard to maintain |
| **EF Core (After)** | Productive, secure, migrations | Performance overhead, can generate inefficient SQL |


### DbContext Configuration
```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Order> Orders { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent API configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasPrecision(18, 2);
            entity.HasIndex(e => e.UserId).HasDatabaseName("IX_Orders_UserId");
            
            // Owned types for value objects
            entity.OwnsOne(o => o.ShippingAddress, addr =>
            {
                addr.Property(a => a.Street).HasColumnName("Shipping_Street");
                addr.OwnsOne(a => a.Coordinates);
            });
            
            // Query filters (soft delete)
            entity.HasQueryFilter(o => !o.IsDeleted);
        });
        
        // Seed data (for development)
        modelBuilder.Entity<User>().HasData(
            new User { Id = Guid.Parse("..."), Email = "admin@example.com" });
    }
    
    // Capture domain events pre-save
    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var domainEvents = ChangeTracker.Entries<IAggregateRoot>()
            .SelectMany(e => e.Entity.PopDomainEvents())
            .ToList();
        
        var result = await base.SaveChangesAsync(ct);
        
        // Publish events after successful save
        foreach (var evt in domainEvents)
            await _eventPublisher.PublishAsync(evt, ct);
        
        return result;
    }
}
```

### Performance Optimization Techniques
```csharp
// 1. Avoid N+1: Use Include/ThenInclude
var orders = await _context.Orders
    .Include(o => o.Items)
    .ThenInclude(i => i.Product)
    .Where(o => o.UserId == userId)
    .ToListAsync();

// 2. Projection: Select only needed fields
var orderSummaries = await _context.Orders
    .Where(o => o.UserId == userId)
    .Select(o => new OrderSummaryDto {
        Id = o.Id,
        Total = o.Total,
        ItemCount = o.Items.Count
    })
    .ToListAsync();

// 3. Split queries for complex graphs (EF Core 5+)
var order = await _context.Orders
    .Include(o => o.Items)
    .Include(o => o.User)
    .AsSplitQuery() // Prevents cartesian explosion
    .FirstOrDefaultAsync(o => o.Id == orderId);

// 4. Raw SQL for complex queries
var topCustomers = await _context.Users
    .FromSqlInterpolated($@"
        SELECT u.*, COUNT(o.Id) as OrderCount 
        FROM Users u
        LEFT JOIN Orders o ON u.Id = o.UserId
        GROUP BY u.Id
        HAVING COUNT(o.Id) > {minOrders}")
    .ToListAsync();

// 5. Compiled queries (for hot paths)
private static readonly Func<AppDbContext, Guid, Task<Order>> 
    _getOrderByIdCompiled = EF.CompileAsyncQuery(
        (AppDbContext ctx, Guid id) => 
            ctx.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == id));

public Task<Order> GetOrderAsync(Guid id) 
    => _getOrderByIdCompiled(_context, id);
```

### Migrations Best Practices
```bash
# Add migration
dotnet ef migrations add AddOrderStatusIndex --context AppDbContext

# Update database
dotnet ef database update --context AppDbContext

# Script for production (idempotent)
dotnet ef migrations script --idempotent --context AppDbContext --output deploy.sql
```

```csharp
// Programmatic migration (for testing)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.Migrate(); // Applies pending migrations
}
```

### Concurrency Handling
```csharp
// Row versioning
public class Order
{
    public Guid Id { get; set; }
    public byte[] RowVersion { get; set; } // Concurrency token
}

modelBuilder.Entity<Order>()
    .Property(e => e.RowVersion)
    .IsRowVersion(); // Maps to SQL Server rowversion

// Handle concurrency exceptions
try 
{
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException ex)
{
    foreach (var entry in ex.Entries)
    {
        var databaseValues = await entry.GetDatabaseValuesAsync();
        if (databaseValues == null)
            throw new NotFoundException(); // Deleted by another user
        
        // Refresh client values or merge changes
        entry.OriginalValues.SetValues(databaseValues);
    }
    // Retry logic or notify user
}
```

### Best Practices
✅ **Use async methods** (`ToListAsync`, `SaveChangesAsync`) to avoid thread pool starvation  
✅ **Disable change tracking** for read-only queries (`.AsNoTracking()`)  
✅ **Batch operations**: `ExecuteUpdateAsync`/`ExecuteDeleteAsync` (EF Core 7+) for bulk changes  
✅ **Use owned types** for value objects to avoid table proliferation  

### Common Pitfalls
❌ Loading entire tables into memory (`.ToList()` without filtering)  
❌ Ignoring connection resiliency (enable retry logic in `AddDbContext`)  
❌ Not configuring decimal precision → SQL truncation errors  

### Interview Insights
> **Q**: "How do you handle database schema changes in a zero-downtime deployment?"  
> **A**:  
> 1. **Expand/contract pattern**:  
>    - Phase 1: Add new column (nullable), deploy code that writes to both old/new  
>    - Phase 2: Backfill data, deploy code that reads from new column  
>    - Phase 3: Remove old column  
> 2. **Use migrations with `--idempotent` scripts** for safe rollouts  
> 3. **Feature flags** to toggle new schema usage  
> Never run destructive migrations (DROP COLUMN) in the same deployment as code changes.

**References**: 
- [EF Core Docs](https://learn.microsoft.com/en-us/ef/core/)
- [EF Core Performance Tips](https://learn.microsoft.com/en-us/ef/core/performance/)
- [Migrations in Team Environments](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/teams)

---

> **Final Tip for Interviews**: Always tie concepts to **business impact**.  
> Example: *"I chose Redis caching to reduce database load during flash sales, cutting P95 latency by 70% and saving $X/month in DB costs."*  
> Architecture decisions should solve real problems—not just follow trends.


### **Explanation Summary**
*   **🧒 For the 15-Year-Old:**
    *   **Before:** Translating every sentence you speak into French manually before talking to a French person.
    *   **After:** Using a translator app. You speak English (C#), the app speaks French (SQL) to the database.
*   **👨‍💻 For the Senior Architect:**
    *   **Performance:** Watch out for **N+1 queries**. Use `.Include()` wisely.
    *   **Tracking:** Use `.AsNoTracking()` for read-only queries to improve speed.
    *   **Migrations:** Treat DB schema changes as code. Use **Idempotent Scripts** for production deployments to ensure safety.

---

## **Final Interview Tip for All Topics**
*   **🧒 For the 15-Year-Old:** "I choose tools that make the app faster and safer, like using a lock on your diary so only you can read it."
*   **👨‍💻 For the Senior Architect:** "I choose architectures that balance **Complexity** vs. **Scalability**. For example, I implemented CQRS only on the Reporting module where read load was 100x the write load, saving 40% on DB costs while keeping the core transactional model simple."

*Document Version: 1.0 | Last Updated: February 2026 | For educational purposes*
