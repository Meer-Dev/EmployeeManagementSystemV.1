## Overall Theme

All year, your “main project” is an **Employee Management Platform (EMP)** that you keep evolving:

- Month 1–2: Simple monolith (small company HR system).  
- Month 3–4: Modular monolith → microservices.  
- Month 5–8: Multi‑tenant, sharded, cached, globally deployed EMP.  
- Month 9–12: Event‑driven, secure, observable, production‑ready EMP with case‑study‑level design docs.

Try to do 5–6 days/week: ~1–1.5h reading/video, ~2–3h coding/diagrams.

***

## Month 1 – C#/.NET, OOP, Fundamentals (EMP v0 → v1)

### Week 1 – C# Refresh + Basic EMP Console App

**Books (this week)**  
- *Clean Code* – Robert C. Martin  
  - Read: Introduction + Chapters 1–2 (“Clean Code”, “Meaningful Names”).

**Daily Coding Focus**

Day 1–2  
- Revisit C# basics:  
  - Types, variables, loops, methods, classes, interfaces, exceptions, collections (List, Dictionary).  
- Mini tasks:  
  - Write a function for each: sum of array, find max, reverse string.  
  - Use GitHub repo `employee-management-journey` and push every day.

Day 3–4  
- Start **EMP Console v0**:  
  - Create classes: `Employee`, `Department`.  
  - Implement in-memory CRUD (List<Employee>).  
  - Menu: Add employee, List employees, Update employee, Delete employee.  
- Apply *Clean Code* ideas:
  - Clear names (`AddEmployee`, not `DoWork`).  
  - Small methods.

Day 5–6  
- Data structures + LeetCode:  
  - Learn arrays, lists, stacks, queues, hash maps, big‑O basics.  
  - LeetCode: 1–2 easy problems/day (Two Sum, Valid Parentheses, Merge Two Sorted Lists).  
- Refactor console EMP:
  - Extract logic into `EmployeeService` class.  
  - Add a very simple `IEmployeeRepository` interface and in‑memory implementation.

***

### Week 2 – OOP, SOLID Basics, Start Design Patterns

**Books (this week)**  
- *Clean Code* – Chapters 3–4 (Functions, Comments).  
- *Design Patterns: Elements of Reusable Object‑Oriented Software* (GoF)  
  - Read the intro + skim Strategy and Factory.

**Daily Coding Focus**

Day 1–2  
- Deepen OOP: inheritance vs composition, interfaces, dependency injection concept.  
- Refactor EMP Console v0 into EMP Console v1:  
  - Use interfaces (`IEmployeeRepository`).  
  - Introduce `EmployeeService` that depends on `IEmployeeRepository` (constructor injection).

Day 3–4  
- Learn SOLID at a high level:  
  - S: each class has one reason to change.  
  - O: open for extension, closed for modification.  
  - D: depend on abstractions (interfaces).  
- Apply to EMP:  
  - Split responsibilities (e.g., separate input/output from core logic).  
  - Ensure `EmployeeService` doesn’t know about console I/O.

Day 5–6  
- Design Pattern focus:
  - Implement a **Factory** (e.g., `EmployeeFactory` that builds Employee with defaults).  
  - Optional: Strategy for different salary calculation or leave rules.  
- LeetCode: continue easy problems daily.

Deliverable end of Week 2:
- EMP Console v1 with:
  - Clean, layered structure.  
  - SOLID‑ish design (at least S, D partially).  
  - 2 small patterns (Factory + maybe Strategy).

***

### Week 3 – ASP.NET Core Web API (EMP v2)

**Books (this week)**  
- *Clean Code* – Chapters 5–7 (Formatting, Objects and Data, Error Handling).  
- Skim online ASP.NET Core docs/tutorial (controllers, routing, DI).

**Daily Coding Focus**

Day 1–2  
- Learn ASP.NET Core basics:
  - Create new Web API project.  
  - Understand Program/Startup, controllers, routing, model binding.  
  - Add a simple `/health` endpoint.

Day 3–4  
- Turn EMP into **Web API**: EMP v2
  - Entities: `Employee`, `Department`.  
  - Controllers: `EmployeesController` with CRUD endpoints.  
  - Use the `EmployeeService` from console version, injected by DI.  
  - Still use in-memory repository for now.

Day 5–6  
- Improve API:
  - Model validation (Data Annotations).  
  - Consistent response types (DTOs instead of entity directly).  
  - Basic error handling middleware or at least proper status codes.  
- LeetCode: 1 easy + 1 medium tree/array problem across these days.

Deliverable end of Week 3:
- ASP.NET Core EMP API v2:
  - CRUD employees via HTTP (tested with Postman or browser).  
  - Clean code principles applied.

***

### Week 4 – Algorithms, EF Core, and Better Architecture

**Books (this week)**  
- Finish main parts of *Clean Code* that matter to you.  
- Skim more GoF patterns (Repository and Template Method ideas).  
- Start *Clean Architecture* – intro + high‑level idea of layers.

**Daily Coding Focus**

Day 1–2  
- Algorithms:
  - Learn basics of recursion, binary search, simple sorting.  
  - LeetCode: 2 easy + 1 medium (binary search, simple DFS/BFS).

Day 3–4  
- Introduce **EF Core** with a local SQL database:
  - Add DbContext (`EmployeeDbContext`).  
  - Configure `Employees` and `Departments` tables.  
  - Migrations and database creation.  
  - Replace in‑memory `IEmployeeRepository` with EF Core implementation.

Day 5–6  
- Architecture refinement:
  - Split solution into projects: `EmployeeManagement.Api`, `EmployeeManagement.Application`, `EmployeeManagement.Domain`, `EmployeeManagement.Infrastructure`.  
  - Ensure API only uses Application layer; Infrastructure contains EF Core.  
  - Clean up naming and namespaces following *Clean Code*.

Deliverable end of Month 1:
- EMP API v2.1:
  - ASP.NET Core, EF Core, layered structure.  
  - `Employee` + `Department` persisted in SQL.  
  - Basic test data seeded.  
- You’ve read most of *Clean Code* core chapters and touched GoF intro.

***

## Month 2 – Clean Architecture, Patterns, Testing (EMP v3)

### Week 5 – Deep SOLID, Clean Architecture, More Patterns

**Books (this week)**  
- *Clean Architecture* – Robert C. Martin:  
  - Read intro + parts on boundaries and layers.  
- *Patterns of Enterprise Application Architecture* – Martin Fowler:  
  - Skim Table of Contents, focus on Repository, Unit of Work, and Layered Architecture.

**Daily Coding Focus**

Day 1–2  
- Review EMP architecture in context of Clean Architecture:
  - Entities (Domain).  
  - Use Cases (Application).  
  - Interfaces/Adapters (Infrastructure).  
- Identify violations (e.g., API leaking DB concerns).

Day 3–4  
- Implement **Repository + Unit of Work** properly:
  - `IEmployeeRepository`, `IDepartmentRepository`, `IUnitOfWork`.  
  - EF Core implementation for them.  
  - Application services use Unit of Work for save operations.

Day 5–6  
- Add one or two more patterns:
  - For example:  
    - **Specification** pattern for filtering employees (active, department, etc.).  
    - **Adapter** to abstract external services (email/SMS service placeholder).

LeetCode:
- 1–2 medium problems this week (trees, graphs basics).

***

### Week 6 – Testing, Refactoring, Open‑Source Contribution

**Books (this week)**  
- Re‑read key *Clean Code* chapters about testing and code smells.  
- *xUnit Test Patterns* (optional, skim) or good blog posts about unit testing in .NET.

**Daily Coding Focus**

Day 1–2  
- Set up unit test project(s) with xUnit or NUnit:  
  - Test `EmployeeService` and any core business logic.  
  - Use Moq (or similar) to mock repositories and external services.

Day 3–4  
- Achieve meaningful coverage:
  - Error cases (e.g., employee not found, invalid data).  
  - Business rules (e.g., cannot delete manager with active reports – if you add that rule).  
- Add some integration tests hitting in‑memory DB or test DB.

Day 5–6  
- Open-source contribution:
  - Find a small .NET repo (bug, typo, or small feature).  
  - Make 1 small PR (even simple).  
- Refactor EMP based on what you learn from others’ code style.

LeetCode:
- 3–4 medium problems this week (graphs, recursion basics).

Deliverable end of Week 6:
- EMP API v3:
  - Clean architecture layers.  
  - Repository + Unit of Work.  
  - Solid unit test suite (30+ tests).  
  - At least one small open‑source contribution.

***

### Week 7 – System Design Basics Applied to EMP

(Starts touching Month 3 content but tuned for your EMP.)

**Books (this week)**  
- Start *Grokking the System Design Interview* (or equivalent).  
- Continue *Patterns of Enterprise Application Architecture* (Layers, Transaction Script vs Domain Model).

**Daily Coding Focus**

Day 1–2  
- Study system design basics:
  - Scalability, availability, consistency, latency, throughput.  
  - Components: load balancer, cache, DB, queue.

Day 3–4  
- Apply to EMP:
  - Draw a **high‑level architecture**:  
    - Client → API Gateway → EMP Service(s) → DB/Cache.  
  - Think multi‑tenant: how to separate data by company.

Day 5–6  
- Small implementation step:  
  - Add Redis (or in‑memory cache as a start) in front of DB for employee reads.  
  - Cache “GetEmployeeById” and “ListEmployeesByDepartment”.

LeetCode:
- Continue 1–2 medium questions, focusing on data structures.

***

### Week 8 – Mini “EMP v3.5” & Review

**Books (this week)**  
- Re‑visit *Clean Code* and *Clean Architecture* sections you struggled with.  
- Skim more PoEAA patterns relevant to EMP (Table Data Gateway, Active Record vs Data Mapper).

**Daily Coding Focus**

Day 1–3  
- Refactor EMP to incorporate what you learned from system design and patterns:
  - Clear separation between read models and write models for heavy endpoints (start hinting at CQRS for later).  
  - Optimize a few queries, ensure indexes exist in DB.

Day 4–6  
- Documentation:
  - Write a solid README: architecture overview, layers, DB schema.  
  - Add simple diagrams (class diagram, architecture diagram).  
  - Note “future scaling steps” (sharding, region‑based deployment).

LeetCode:
- 2–3 medium problems, focusing on trees/graphs (helpful later in system design interviews).

Deliverable end of Month 2:
- EMP API v3.5:
  - Clean architecture, patterns, tests, caching basics.  
  - Good documentation and diagrams.  
  - You have solid foundations to move into full system design, microservices, and massive scale for the remaining months.

***

If you tell me approximately **how many hours/day** you can realistically commit (2, 4, 6+), I can shrink or expand this into a **daily checklist for next week** (e.g., “Monday: read X pages, finish Y endpoints, solve Z LeetCode problems”).
## Month 3 – System Design Fundamentals (for HR at scale)

**Core Goal:** Understand system design components and apply them to an EMP.

**Books**

- *Grokking the System Design Interview* (or similar online material).  
- Read selected chapters from *Clean Architecture* around boundaries and layers.

**What to Learn**

- Concepts: scalability, availability, consistency, latency, throughput.  
- Components: load balancer, API gateway, cache, database, message queue, CDN.  
- Patterns: basic CQRS idea, eventual consistency.  
- How multi‑tenant SaaS is designed (HR SaaS like Workday/BambooHR as inspiration).

**EMP Focus**

- Design **EMP v2 (Monolith but scalable)**:
  - Multi‑tenant (Company, Employee, Role, Permission).  
  - Think for 10B employees:  
    - How you partition data (company ID, region).  
    - What needs strong consistency (payroll) vs eventual (reports).
- Draw diagrams (use draw.io):  
  - Request flow: client → API gateway → EMP service → DB/cache.  
  - Data model: Company, Employee, Department, Manager, Leave, Shift.

**Practice**

- Do 2–3 simple system design exercises: URL shortener, news feed, file storage.  
- Implement a basic **EMP v2** with a real SQL database beneath your API.

***

## Month 4 – Microservices & Distributed Systems (EMP Services)

**Core Goal:** Break EMP into services and run them with containers.

**Books**

- *Microservices Patterns* – Chris Richardson.  
- *Hands‑On Microservices with C# and .NET* (optional but helpful).

**What to Learn**

- Monolith vs microservices, service boundaries and data per service.  
- Inter‑service communication: REST, messaging, contracts.  
- Docker: images, containers, networks, volumes.  
- Intro to Kubernetes: pods, deployments, services.

**EMP Focus**

Split EMP into microservices:

- **Identity/Company Service** (companies, admins).  
- **Employee Service** (employees, managers, org structure).  
- Optional: **Leave/Attendance Service**.

Each service:

- Has its own database.  
- Exposes REST endpoints.  
- Runs in its own Docker container.

Create a **local stack**:

- API gateway (simple reverse proxy or YARP).  
- Services in Docker Compose.  
- Shared message broker (if you already feel ready).

***

## Month 5 – Relational Data, SQL, and HR Data Models

**Core Goal:** Be solid in relational modeling and performant SQL for HR use‑cases.

**Books**

- *Database System Concepts* – Silberschatz et al.  
- *Pro Entity Framework Core* or *EF Core in Action*.

**What to Learn**

- ER modeling, normalization, constraints, indexing.  
- Transactions, isolation levels (important for payroll, updates).  
- EF Core advanced mapping, migrations, LINQ performance tips.

**EMP Focus**

- Design a **rich HR schema**:
  - Multi‑tenant (Tenant/Company ID everywhere).  
  - Employees, Contracts, Salaries, Departments, Locations, Devices, Audit logs.  
  - History tables (for salary changes, department changes).
- Implement:
  - Complex queries:  
    - Count employees by region.  
    - Top overtime hours by department.  
    - Monthly turnover rate.
  - Stored procedures or efficient raw SQL where needed.

Start thinking: “If there are billions of employees, how do I partition this schema across regions and shards?”

***

## Month 6 – NoSQL & Time‑Series / Logs for EMP

**Core Goal:** Combine SQL + NoSQL for huge data volume (logs, shifts, events).

**Books**

- *Designing Data‑Intensive Applications* – Martin Kleppmann.  
- Selected DynamoDB/Cassandra online docs/tutorials to understand models.

**What to Learn**

- Types of NoSQL (document, key‑value, column‑family, graph) and CAP.  
- Denormalization, pre‑aggregated views, partition keys.  
- Event logs and audit trails.

**EMP Focus**

Add NoSQL components:

- Document DB (MongoDB/Cosmos DB) for:  
  - Employee profiles with flexible attributes.  
  - Policy documents, attachments.
- Wide‑column or key‑value for:  
  - Attendance/shift logs (massive, time‑series).  
  - Login/audit events.

Examples:

- Store millions of daily punch‑in/out events in a wide‑column or time‑series style.  
- Build an API to query last 90 days of employee activity efficiently.

Design for 10B employees:

- Partition by company + region + time.  
- Use write‑optimized stores for logs, SQL for core entities.

***

## Month 7 – Cloud Infrastructure (Global EMP)

**Core Goal:** Deploy EMP to cloud with multi‑region in mind.

**Books**

- *Architecting for the Cloud: AWS Best Practices* (AWS whitepaper).  
- *Azure for Architects* (if going Azure).

**What to Learn**

- Core cloud: VPC/VNet, subnets, security groups, load balancers.  
- Managed DBs (RDS/Azure SQL), object storage (S3/Blob).  
- Basics of multi‑region deployments (active‑active vs active‑passive).

**EMP Focus**

- Deploy EMP to the cloud:  
  - Use a managed relational DB for core HR data.  
  - Use object storage for documents (contracts, payslips).  
  - Put services behind a managed load balancer or API gateway.
- Start designing **region‑aware EMP**:
  - Example: APAC cluster, EMEA cluster, Americas cluster.  
  - Users routed to nearest region, data residency compliance.

***

## Month 8 – Scaling & Performance for “10B Employees”

**Core Goal:** Learn serious scaling techniques and apply them to EMP.

**Books**

- Continue *Designing Data‑Intensive Applications* (chapters on replication, sharding, streams).  
- Any good performance tuning book/material for .NET (e.g., *Pro .NET Memory Management*).

**What to Learn**

- Caching: Redis, cache‑aside vs write‑through; multi‑level caches.  
- Sharding and partitioning strategies: by company, geography, hash of employee ID.  
- Read replicas, CQRS for heavy read scenarios.  
- Load testing and profiling.

**EMP Focus**

- Add **multi‑layer caching**:
  - Employee profile cache (Redis).  
  - Frequently used org charts cached per company.
- Implement **sharding strategy** design:  
  - For example:  
    - Shard databases by company ranges or region.  
    - Directory service that maps company ID → shard.
- Load test:
  - Simulate large companies (millions of employees each).  
  - Measure API latency and throughput.  
  - Optimize: indexes, batching, caching, connection pooling.

Think: “How would I store and query data if total employees = order of 10B?”

***

## Month 9 – Advanced Architecture Patterns (Event‑Driven EMP)

**Core Goal:** Use events to decouple modules and scale HR features.

**Books**

- *Enterprise Integration Patterns* – Gregor Hohpe & Bobby Woolf.  
- Re‑read relevant parts of *Microservices Patterns* (Saga, CQRS, messaging).

**What to Learn**

- Event‑driven architecture, event sourcing basics, pub/sub.  
- Patterns: Saga, Outbox, Circuit Breaker, Bulkhead.  
- Consistency models, idempotent consumers.

**EMP Focus**

Event‑driven EMP features:

- EmployeeCreated, EmployeeUpdated, EmployeeTerminated events.  
- Downstream services:  
  - Payroll service.  
  - Access control/badge provisioning.  
  - Reporting/analytics.
- Implement:
  - A Saga for “Onboard Employee”: create in core DB, give access in other systems, send welcome emails.  
  - Outbox pattern to avoid losing events.  
  - Circuit breaker around external dependencies (e.g., payroll gateway).

***

## Month 10 – Security & Compliance for HR Data

**Core Goal:** Make EMP safe for highly sensitive personal data at global scale.

**Books**

- *OWASP Top 10 documentation* (online).  
- *Security Engineering* – Ross Anderson (read selected chapters).  

**What to Learn**

- OWASP Top 10, secure coding, secrets management.  
- AuthN/AuthZ: JWT, OAuth2/OIDC, role‑based & attribute‑based access.  
- Data protection: encryption at rest, encryption in transit, PII handling.  
- Basics of regulations: GDPR‑like concepts (data residency, right to be forgotten).

**EMP Focus**

- Implement robust auth and fine‑grained authorization:
  - Roles: Global Admin, Company Admin, Manager, Employee.  
  - Permissions per resource, per tenant.
- Add:
  - Secure password storage / external IdP integration.  
  - Audit logging for sensitive actions (view salary, change role).  
  - Data masking where needed (e.g., show partial SSN/ID).
- Threat‑model the EMP:
  - What if token stolen?  
  - What if DB compromised?  
  - How to detect abuse?

***

## Month 11 – Observability & DevOps for EMP

**Core Goal:** Operate EMP as a serious production system.

**Books**

- *Site Reliability Engineering* – Google SRE Book (selected chapters).  
- *Infrastructure as Code* – Kief Morris.

**What to Learn**

- Observability: logs, metrics, traces, SLIs/SLOs.  
- Alerting, dashboards, golden signals (latency, errors, traffic, saturation).  
- CI/CD pipelines, blue/green or canary deployments.  
- Infrastructure as Code (Terraform, Bicep, or CloudFormation).

**EMP Focus**

- Add full observability:
  - Correlation IDs through all EMP services.  
  - Dashboards for:  
    - Requests/sec, error rate, p95 latency per service.  
    - Background job queue length.  
    - DB and cache metrics.
- CI/CD:
  - Build: compile, run tests, static analysis.  
  - Deploy: dev → staging → prod environment.
- Infrastructure as Code:
  - Describe one full environment (network, DBs, caches, services) as code.  
  - Recreate environment from scratch using scripts.

***

## Month 12 – Case Studies, Capstone EMP at “Planet Scale”

**Core Goal:** Synthesize everything into a well‑documented, large‑scale EMP architecture and implementation slice.

**Books / Material**

- Revisit *Designing Data‑Intensive Applications* for overall patterns.  
- Martin Fowler’s online Software Architecture Guide for architectural trade‑offs.

**What to Do**

- Study 2–3 large‑scale systems (e.g., multi‑tenant SaaS, social networks) and map ideas to EMP:
  - How they shard users/tenants.  
  - How they handle regional data, consistency, and failover.
- Design your **Planet‑Scale EMP** on paper:
  - Regions, shards, core services, data flows.  
  - Read/write paths, caching layers, queues, analytics pipeline.  
  - Failure scenarios and recovery.

**Capstone Implementation**

You don’t have to build everything; build a **vertical slice** deeply:

- Example slice: “Employee Onboarding at 10B scale”:
  - API → Identity/Company Service → Employee Service → Event Bus → Provisioning Service → Audit & Analytics.  
  - Sharded SQL + NoSQL for logs.  
  - Caching, retries, saga, observability, security.

Document clearly:

- Requirements and constraints (multi‑tenant, global, 10B employees).  
- Architecture diagram(s) and data models.  
- Scaling strategies (sharding, caching, CQRS, multi‑region).  
- Trade‑offs you made (consistency vs availability, complexity vs speed of development).

