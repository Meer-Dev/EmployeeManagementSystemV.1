# Core runtime & toolchain

1. .NET CLI / SDK / runtime

   * `dotnet build` vs `dotnet run` vs `dotnet publish` — what each does, outputs, artifacts.
   * How IL, assemblies, and metadata work.
   * How `dotnet publish` produces self-contained vs framework-dependent outputs.
2. JIT / AOT / ReadyToRun / NativeAOT

   * When JIT compiles, when AOT is used, tradeoffs.
   * Startup vs steady-state performance implications.
3. CLR internals

   * Garbage Collector modes (Workstation/Server), generational GC, LOH, pinning, GC pressure.
   * Assembly loading, `AssemblyLoadContext`, unloading assemblies safely.
4. Threading & async internals

   * ThreadPool behavior, starvation, thread injection, `ConfigureAwait(false)` consequences.
   * Task scheduling, synchronization contexts (ASP.NET Core vs UI).
5. Memory diagnostics

   * How to capture and analyze memory dumps, GC heap dumps, find memory leaks.

# C# language (advanced)

1. Latest language features (records, nullable reference types, pattern matching).
2. Value types vs reference types, boxing/unboxing, `ref struct`, `Span<T>`, `Memory<T>`.
3. Unsafe code, pointers, stackalloc — when and why.
4. `IAsyncEnumerable<T>` streaming, cancellation propagation.
5. Compiler-generated code: closures, iterator state machines, async state machines — how they affect performance/debugging.
6. Heap vs Stack in detail
7. Classes and its types when to use what, how its used.

# ASP.NET Core & Web API internals

1. Kestrel internals and HTTP pipeline.
2. Middleware pipeline: ordering, short-circuiting, exception flows.
3. Hosting models: Generic Host vs Web Host, life cycle hooks.
4. Controllers vs Minimal APIs: when to use each, tradeoffs.
5. Model binding & validation pipeline internals.
6. Response buffering, streaming, chunked responses.

# Dependency Injection & lifetimes (deep)

1. Service lifetimes: Singleton, Scoped, Transient — exact lifecycle semantics.
2. Problems: Transient-in-Singleton, Scoped-in-Singleton — causes and diagnostics.
3. `IServiceProvider`, `IServiceScopeFactory`, creating scopes manually inside singletons — safe patterns and pitfalls.
4. Factories (`AddSingleton<T>(sp => ...)`) and resolving scoped dependencies correctly.
5. Circular dependencies detection and solutions.

# Architecture patterns & design

1. Clean Architecture / Hexagonal / Onion — responsibilities per layer, dependency rules.
2. CQRS — when to separate reads/writes, data duplication, consistency models.
3. Event sourcing basics vs relational CRUD — pros/cons.
4. Command / Query Handler design (MediatR) — pipeline behaviors, cross-cutting concerns.
5. Repository pattern: pros/cons in EF Core world; alternatives and when to keep it thin vs remove it.
6. Unit of Work vs DbContext — mapping and lifetime.
7. API versioning strategies and migration plans.

# Entity Framework Core (expert)

1. Change Tracker internals — keys, snapshots, identity resolution.
2. `AsNoTracking()` and `AsNoTrackingWithIdentityResolution()` — differences and when to use.
3. Tracking vs no-tracking performance & memory cost.
4. Query translation: what LINQ translates and what runs client-side. How to detect client evaluation.
5. Compiled queries and `EF.CompileQuery`.
6. Query splitting vs `Include()` — single SQL with joins vs multiple queries; pros/cons for cartesian explosion.
7. Eager vs lazy vs explicit loading — when and why.
8. Concurrency tokens / optimistic concurrency — `RowVersion`, handling `DbUpdateConcurrencyException`.
9. Transactions in EF Core
   * `SaveChanges()` implicit transaction behavior.
   * Explicit `IDbContextTransaction`, `TransactionScope`.
   * When to use distributed transactions (and alternatives).
10. Execution strategies and connection resiliency (retries).
11. Migrations: design, scripts, one-time vs repeatable scripts, rollbacks, data migrations.
12. Performance: SQL parameterization, batching, avoiding N+1, projection to DTOs (`Select` into DTO).
13. Raw SQL: `FromSqlRaw`, mapping to entities vs unmapped types.
14. DbContext pooling — benefits and gotchas.
15. Testing EF Core: InMemory vs SQLite vs real SQL for integration tests; pitfalls of InMemory provider.

# SQL Server & relational DB expertise

1. Execution plans, indexes (clustered vs nonclustered), covering indexes, include columns.
2. Index fragmentation, rebuild vs reorganize, statistics.
3. Transactions and isolation levels (Read Uncommitted/Read Committed/Repeatable Read/Snapshot/Serializable) and their effects.
4. Deadlocks: detection and resolution strategies.
5. Query tuning: `EXPLAIN`, parameter sniffing, query hints, tempdb considerations.
6. Partitioning, sharding, read replicas, horizontal scaling patterns.
7. Temporal tables, JSON support, computed columns, CLR SQL integration.
8. Backups, restores, point-in-time recovery, restore testing.

# Security / Auth / AuthZ (important — you said next week)

1. Authentication fundamentals: JWT, opaque tokens, sessions, cookies.
2. OAuth2 flows (authorization code, client credentials, refresh tokens) and OpenID Connect basics.
3. IdentityServer / Duende alternatives, ASP.NET Core Identity: when to use.
4. Token best practices: signing, validation, rotation, revocation.
5. Secure storage of secrets — Azure Key Vault, HashiCorp Vault, AWS Secrets Manager.
6. Common attacks and mitigations: XSS, CSRF, SQL Injection, open redirects, clickjacking.
7. HTTPS, HSTS, CSP, CORS policy configuration — strict minimal privileges.
8. Claims-based authorization and policies; resource-based authorization.
9. Multi-tenant auth patterns and isolation.
10. Rate limiting and abuse protection.

# Logging, observability & diagnostics

1. Structured logging fundamentals (Serilog): sinks, enrichers, destructuring, output templates.
2. Correlation IDs: generate, propagate, log, and use in traces.
3. Logging at startup — capturing startup failures and ensuring logs are flushed.
4. Log aggregation: Seq, ELK stack, Splunk, Azure Monitor, Application Insights.
5. Distributed tracing: OpenTelemetry, Jaeger — traces, spans, context propagation across services.
6. Metrics: Prometheus metrics instrumentation, counters, histograms.
7. Health checks: readiness vs liveness, custom checks.
8. Exception handling strategies: global exception middleware, mapping exceptions to HTTP codes, hiding internals.
9. Perf profiling: `dotnet-trace`, `dotnet-counters`, CPU/memory flame graphs.
10. Live debugging tools and remote diagnostics.

# Resiliency & reliability patterns

1. Retry policies, exponential backoff, jitter (Polly).
2. Circuit breaker, bulkhead isolation, fallback strategies.
3. Idempotency design for APIs.
4. Rate limiting, throttling, queueing patterns.
5. Graceful shutdown, draining connections, background tasks finish.

# Serialization & data contracts

1. `System.Text.Json` vs `Newtonsoft.Json`: features, performance, polymorphism.
2. DTO design: command/request vs domain models, version tolerant serialization.
3. Binary formats and gRPC: when to choose gRPC vs REST.
4. Schema evolution and backward compatibility.

# Caching & performance

1. In-process caching vs distributed caching (MemoryCache, Redis).
2. Cache invalidation strategies: TTL, cache-aside, write-through, write-behind.
3. Response caching, ETags, conditional GET.
4. CDN use for static assets and APIs.
5. Profiling hotspots; measuring real user latencies vs synthetic.

# Concurrency & consistency

1. Optimistic vs pessimistic concurrency; use cases.
2. Eventual consistency models and tradeoffs.
3. Designing for concurrent updates (merging, last-write-wins, CRDT basics).
4. Multi-write / multi-region coordination patterns.

# Background processing & workers

1. `IHostedService` / `BackgroundService` vs external job systems.
2. Hangfire, Quartz.NET — tradeoffs and persistence considerations.
3. Durable functions, serverless alternatives.
4. Ensuring idempotence and retries for background jobs.

# Testing strategy (senior)

1. Unit tests, integration tests, contract tests, end-to-end tests.
2. Test doubles: mocking vs fakes vs in-memory vs test containers.
3. Testing EF Core correctly (migrations, seed data, isolation).
4. API contract testing with Postman / Pact.
5. Load testing and soak testing (k6, JMeter).
6. CI test pipeline: parallel test runs, flaky test detection, test coverage thresholds.

# CI/CD, deployment & infra

1. Dockerizing .NET apps: multistage builds, layer caching, security scanning.
2. Kubernetes basics: deployments, services, ingress, ConfigMaps, Secrets, probes.
3. Helm charts, manifests, GitOps (Flux/ArgoCD).
4. Cloud deployment patterns: Azure App Service vs Kubernetes vs AKS vs AWS ECS/EKS.
5. Blue/Green and Canary deployments; feature flags.
6. Infrastructure as Code: Terraform vs ARM vs Bicep.
7. Build pipelines: GitHub Actions, Azure DevOps — artifact signing, security scanning, dependency scanning.

# Observability in production

1. Centralized logging, retention policies, cost controls.
2. Alerting rules: reducing noise, alert fatigue, SLO/SLI/SLA definitions.
3. Runbooks for incidents, Postmortems and RCA practice.
4. Chaos engineering basics: fault injection, verifying resiliency assumptions.

# API design & developer experience

1. REST best practices: status codes, hypermedia if needed, error formats.
2. API pagination, filtering, sorting, search best practices.
3. OpenAPI/Swagger documentation and generation.
4. Client SDK generation and API compatibility guarantees.
5. Rate limiting and API keys.

# Data engineering & messaging

1. Message brokers: RabbitMQ, Kafka — semantics (at most once / at least once / exactly once).
2. Consumer groups, partitioning, compaction, retention.
3. Schema registry and Avro/Protobuf.
4. Event-driven integration patterns, sagas for long-running transactions.

# Dev ergonomics & code quality

1. Static analysis tools, Roslyn analyzers, StyleCop, SonarQube.
2. Code reviews: checklist items for maintainability, security, performance.
3. Design docs and ADRs (Architecture Decision Records).
4. Refactoring techniques, technical debt management.

# Hands-on tasks / exercises (implement and explain)

1. Implement an Employee API with:

   * Clean Architecture, EF Core, CQRS (MediatR), FluentValidation, AutoMapper, Serilog.
   * Add AsNoTracking for read endpoints; measure memory before/after under load.
2. Add JWT auth + role-based and policy-based authorization.
3. Implement a scoped service used inside a singleton properly using `IServiceScopeFactory`.
4. Create a failing startup scenario (throw inside Program.cs) and verify Serilog captures and flushes the startup error.
5. Implement paging, filtering, sorting with safe SQL translation and tests.
6. Add `IHostedService` background worker that processes messages from a queue (simulate with in-memory queue).
7. Add retry + circuit breaker (Polly) for outbound HTTP calls and prove behavior in failure scenarios.
8. Add distributed tracing (OpenTelemetry) across a 2-service demo app; show correlated logs and traces.
9. Create a migration that requires data transformation (schema + data) and write a safe deployment plan.
10. Load test read vs write endpoints, identify bottlenecks, tune DB indexes, and repeat.

# Interview-style questions you must answer confidently

1. Explain precisely why Scoped->Singleton dependency is wrong. Show a minimal repro and fix it.
2. What exactly happens when you call `SaveChanges()` in EF Core? List steps and exceptions that can occur.
3. How does `AsNoTracking()` reduce memory use? Show memory difference with a sample of 100k rows.
4. How would you design authentication for a public API consumed by mobile apps with offline sync?
5. Design an API rate limiter that is distributed (multiple instances) with per-tenant limits.
6. Explain how to do zero-downtime DB migrations that add a non-nullable column.
7. When would you choose gRPC over REST? Give 3 production scenarios.
8. Show how to implement end-to-end correlation IDs across HTTP + background jobs + DB entries.
9. Explain how to debug a production high CPU issue in a .NET app.
10. How to secure secrets in CI pipeline and avoid leaking them in logs?

# Anti-patterns & red flags to spot

1. Heavy business logic in controllers.
2. Overuse of repositories when EF Core already provides a unit of work.
3. Long synchronous blocking calls in request pipeline.
4. Capturing scoped services in static/singleton state.
5. Swallowing exceptions and returning 200 with error payloads.
6. Using InMemory EF provider for integration test parity with prod.

## MUST MASTER THESE :

GC + memory pressure
Scoped vs Singleton lifecycle correctness
EF Core tracking vs no tracking
SQL indexing + execution plans
Isolation levels + deadlocks
OAuth2 flows correctly implemented
Retry + circuit breaker strategy
Distributed tracing
Zero-downtime migrations
Production debugging workflow

---
🧱 PHASE 1 (Weeks 1–2): Runtime & Concurrency Mastery

Goal: Understand .NET at a mechanical level.

You must be able to answer:

Why did memory spike?

Why is CPU 100%?

Why is latency inconsistent?

You will:
1️⃣ Write allocation-heavy code

Then:

Measure with dotnet-counters

Capture dump

Inspect GC heap

Identify large object heap usage

2️⃣ Create thread pool starvation

Then:

Measure queued work items

Observe injection delay

Fix with async properly

3️⃣ Inspect async state machine IL

Understand:

Hidden allocations

Closure captures

Struct vs class behavior

By end of Phase 1:
You think in terms of:

Allocation rate

GC pause time

Thread scheduling

Most developers never reach this level.

🧱 PHASE 2 (Weeks 3–4): ASP.NET Core + DI Internals

Goal: Master request lifecycle and lifetime correctness.

You must understand:

From socket → Kestrel → middleware → endpoint → response flush

You will:

Build:

Custom correlation middleware

Timing middleware

Exception middleware

Break:

Scoped service inside singleton

Captive dependency memory leak

Blocking call inside request pipeline

Fix:

Using:

IServiceScopeFactory

Correct async usage

Proper lifetime design

By end:
You instinctively see lifetime bugs.

🧱 PHASE 3 (Weeks 5–6): EF Core + SQL at Production Level

This is where most “senior” devs collapse.

You will:

Measure memory difference:

100k rows:

Tracking

AsNoTracking

Projection to DTO

Quantify difference.

Cause:

N+1 query explosion

Deadlock

Lock escalation

Then fix them.

Analyze:

Execution plans

Missing indexes

Parameter sniffing

By end:
You stop trusting EF blindly.
You understand what SQL actually runs.

🧱 PHASE 4 (Week 7): Security, Resiliency, Distributed Thinking

You will implement:

JWT auth correctly

Refresh token rotation

Distributed rate limiter (Redis)

Polly retry with jitter

Circuit breaker

Then simulate:

Downstream API failure

Token replay

Rate limiter under load

You’ll learn:

Resiliency is controlled failure.

🧱 PHASE 5 (Week 8): Observability & Failure Simulation

This is elite territory.

You will:

Add OpenTelemetry

Correlate logs across 2 services

Propagate correlation ID to DB

Capture memory dump

Diagnose CPU spike

Diagnose thread starvation

You must be able to answer:

“If production CPU is 95%, what do you do first?”

🧱 PHASE 6 (Optional Weeks 9–10): Architecture Depth

Only after fundamentals are solid.

You will:

Implement Clean Architecture properly

Add CQRS where justified

Add MediatR pipeline behaviors

Implement safe zero-downtime migration

And explain when NOT to use them.

🧠 Daily Training Structure (Important)

Since you have full-time intensity:

Daily 6–8 hours:

2 hrs – Theory reading (docs + source code)
3 hrs – Implementation
1 hr – Breaking things intentionally
1 hr – Debugging and measuring
1 hr – Writing summary of what you learned

Writing summaries is critical.

If you can’t explain it clearly, you don’t understand it.

🧨 Brutal Truth About Elite Level

Elite engineers:

Think in tradeoffs

Measure before optimizing

Know failure modes

Don’t blindly apply patterns

Understand runtime cost of abstractions

They don’t just know:
“What is CQRS?”

They know:
“When CQRS will hurt you.”
