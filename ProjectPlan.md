Here is the **Ultimate Enterprise EMS Master Prompt**. This is designed to be saved as `PROJECT_SPEC.md` in your root directory and referenced by Cursor/Copilot using `@PROJECT_SPEC.md`.

It includes the **Backend Normalization Scan**, **SSE Implementation**, **Enterprise Feature Expansion**, **In-Depth Request Flow**, and **Deployment Guide**.

---

# 📂 Save as: `PROJECT_SPEC.md`

```markdown
# 🚀 PROJECT SPECIFICATION: Enterprise Employee Management System (EMS)
**Version:** 2.0 (Enterprise Grade)
**Role:** Senior Full-Stack Architect & Developer
**Stack:** Angular 18+ (Frontend), .NET 8/9 (Backend), PostgreSQL/SQL Server, Docker

---

## ⚠️ CRITICAL PRE-FLIGHT INSTRUCTIONS (READ FIRST)
1.  **SCAN BACKEND FIRST:** Before generating ANY frontend code, you MUST scan the entire `/Backend` directory.
    *   **Identify Entities:** If only `Employee` exists, you MUST propose and implement normalization (Departments, Roles, Policies, AuditLogs, Notifications).
    *   **Verify SSE:** Check if Server-Sent Events (SSE) endpoints exist. If not, create them for CSV imports/exports and real-time notifications.
    *   **Verify Policies:** Ensure Policy-Based Access Control (PBAC) exists beyond simple Roles (e.g., `CanDeleteEmployeeInOwnDepartment`).
2.  **SYNC CHECK:** Ensure Frontend Models match Backend DTOs exactly. No `any` types.
3.  **NORMALIZATION:** A production app is not single-entity. Ensure relational data (Employee ↔ Department ↔ Role ↔ AuditLog) is modeled correctly in both DB and NgRx State.

---

## 1️⃣ ENTERPRISE FEATURE CHECKLIST (ADD MISSING ITEMS)
*Do not proceed until these are accounted for in architecture:*
- [ ] **Audit Logging:** Every write operation (POST/PUT/DELETE) must log `Who, What, When, OldValue, NewValue, IP, CorrelationID`.
- [ ] **Department Hierarchy:** Tree-structure support (Parent/Child Departments).
- [ ] **Policy-Based Access Control (PBAC):** Beyond Roles. E.g., "HR Manager can only delete employees in their assigned Department".
- [ ] **Real-Time Notifications:** SSE channel for system alerts, approval requests, and task completion.
- [ ] **Bulk Operations:** CSV Import/Export with **SSE Progress Tracking** (0-100% stream).
- [ ] **Advanced Reporting:** Dashboard with aggregated metrics (Headcount, Turnover, Dept Distribution) using cached views.
- [ ] **Data Retention:** Soft deletes (`IsDeleted` flag) + Archive strategy.
- [ ] **Session Management:** Ability to revoke active sessions from Admin Panel.
- [ ] **Multi-Factor Authentication (MFA):** Placeholder/Integration ready for Admin accounts.
- [ ] **Localization (i18n):** Prepare structure for multiple languages (EN/ES/FR).

---

## 2️⃣ ARCHITECTURE & NORMALIZATION RULES

### 2.1 Backend Normalization (Scan & Fix)
*If backend is flat, refactor to:*
```
Employees (Id, FirstName, LastName, DepartmentId, RoleId, IsDeleted)
Departments (Id, Name, ParentDepartmentId, CostCenter)
Roles (Id, Name, IsSystemRole)
Permissions (Id, Name, Resource, Action)
RolePermissions (RoleId, PermissionId)
UserRoles (EmployeeId, RoleId)
AuditLogs (Id, EmployeeId, Action, Timestamp, CorrelationId, Details)
Notifications (Id, EmployeeId, Message, IsRead, Type)
```
- **Constraint:** Use Foreign Keys with Cascade Delete where appropriate (but Soft Delete for Employees).
- **Indexing:** Add indexes on `Email`, `DepartmentId`, `IsDeleted`, `CreatedAt`.

### 2.2 Frontend State Normalization (NgRx Entity)
*Do not store arrays of objects. Normalize state:*
```typescript
// Employee State
entities: { [id: string]: Employee }
ids: string[]
// Department State (Normalized separately)
departments: { [id: string]: Department }
// Join selectors
selectEmployeesWithDept = createSelector(..., (employees, depts) => ...)
```

### 2.3 Server-Sent Events (SSE) Architecture
**Backend:**
- Endpoint: `GET /api/stream/notifications` (Requires Auth)
- Endpoint: `GET /api/stream/import-progress/{taskId}`
- Service: `ISseService` managing `ChannelMapper` for specific user channels.

**Frontend:**
- Service: `SseService` (Singleton in Core)
- Implementation: Use `EventSource` wrapped in RxJS Observable.
- Reconnection: Exponential backoff on disconnect.
- Store Integration: SSE events dispatch NgRx Actions (e.g., `NotificationReceived`, `ImportProgressUpdated`).

---

## 3️⃣ IN-DEPTH REQUEST FLOW (END-TO-END)
*Every API interaction MUST follow this flow. Do not skip steps.*

### 3.1 Write Operation Flow (e.g., Create Employee)
1.  **User Action:** User clicks "Save" on Form.
2.  **Component Validation:** Reactive Forms check validity (Client-side).
3.  **Dispatch:** Component dispatches `CreateEmployeeAction({ payload })`.
4.  **Effect Trigger:** `CreateEmployeeEffect` listens.
5.  **Pre-Flight Check:** Effect checks `selectIsOnline` and `selectIsRateLimited`.
6.  **Service Call:** Effect calls `EmployeeService.create()`.
7.  **Interceptor Chain (Request):**
    *   `CorrelationIdInterceptor`: Generates/Attaches `X-Correlation-ID`.
    *   `AuthInterceptor`: Retrieves Token from NgRx Store → Adds `Authorization: Bearer`.
    *   `IdempotencyInterceptor`: Adds `Idempotency-Key` (UUID) for POST.
    *   `LoggingInterceptor`: Logs Request Start + Payload (sanitized).
8.  **Network:** HTTP POST sent to `https://api.domain.com/api/employees`.
9.  **Backend Middleware:**
    *   **Global Exception Filter:** Catches unhandled errors.
    *   **Correlation Middleware:** Extracts `X-Correlation-ID` → Adds to `ILogger Scope`.
    *   **Auth Middleware:** Validates JWT → Sets `HttpContext.User`.
    *   **Policy Handler:** Checks `CanCreateEmployee` policy (PBAC).
10. **Controller:** Receives DTO → Validates (FluentValidation) → Maps to Entity.
11. **Service Layer:** Business Logic → Checks Department Quotas → Checks Duplicate Email.
12. **Repository/DB:** `INSERT INTO Employees` → `INSERT INTO AuditLogs` (Transaction).
13. **Response Construction:** Wraps result in `ApiResponse<T>` → Adds `X-Correlation-ID` header.
14. **Interceptor Chain (Response):**
    *   `LoggingInterceptor`: Logs Response Status + Duration.
    *   `ErrorInterceptor`: Checks Status (200/201 = Pass, 4xx/5xx = Catch).
    *   `UnwrapInterceptor`: Extracts `data` from `ApiResponse` wrapper.
15. **Effect Success:** `CreateEmployeeEffect` receives data → Dispatches `CreateEmployeeSuccessAction`.
16. **Reducer:** Updates NgRx Store (Normalized Entity Insert).
17. **Side Effects:**
    *   Dispatch `ShowNotificationAction` ("Employee Created").
    *   Trigger SSE Push to Admins ("New Employee Added").
18. **Component Render:** `async` pipe detects Store change → UI updates instantly.

### 3.2 SSE Flow (e.g., CSV Import Progress)
1.  **User Action:** Uploads CSV → Clicks "Import".
2.  **Frontend:** Dispatches `StartImportAction`.
3.  **Backend:** Returns `202 Accepted` with `taskId`.
4.  **Frontend:** Opens `EventSource` to `/api/stream/import-progress/{taskId}`.
5.  **Backend Worker:** Processes CSV row-by-row.
6.  **Backend SSE:** Pushes `{ progress: 10, status: 'Processing' }` every 500ms.
7.  **Frontend SSE Service:** Receives message → Dispatches `ImportProgressUpdateAction`.
8.  **Store:** Updates `importProgress` state.
9.  **UI:** Progress Bar updates reactively via `async` pipe.
10. **Completion:** Backend sends `event: complete` → Frontend closes `EventSource` → Reloads Employee List.

---

## 4️⃣ SECURITY & COMPLIANCE (ENTERPRISE GRADE)
- **Headers:** Enforce `Strict-Transport-Security`, `X-Content-Type-Options`, `Content-Security-Policy`.
- **CORS:** Strict whitelist (No `*`). Allow credentials.
- **Rate Limiting:** Sliding window per IP + per User ID.
- **Data Protection:** Encrypt PII (Email, Phone) at rest (TDE or Column Encryption).
- **Audit:** No hard deletes. `IsDeleted` flag + `DeletedAt` + `DeletedBy`.
- **Tokens:** Access Token (15min), Refresh Token (7 days, Rotating).
- **Input:** Sanitize all HTML inputs. Validate MIME types for uploads.

---

## 5️⃣ DEPLOYMENT GUIDE (DEVOPS)
*Generate Dockerfiles and Compose based on this spec.*

### 5.1 Containerization
- **Frontend:** Multi-stage build (Node → Nginx Alpine).
- **Backend:** .NET Slim runtime image.
- **Database:** PostgreSQL official image.
- **Redis:** For caching and SignalR/SSE backplane (if scaled).

### 5.2 Docker Compose (Local/Dev)
```yaml
version: '3.8'
services:
  web:
    build: ./frontend
    ports: ["80:80"]
    depends_on: [api]
  api:
    build: ./backend
    environment:
      - ConnectionStrings__Default=Host=db;Database=ems;Username=postgres;Password=secure
      - Jwt__Key=${JWT_SECRET}
    depends_on: [db, redis]
  db:
    image: postgres:15-alpine
    volumes: [pgdata:/var/lib/postgresql/data]
  redis:
    image: redis:alpine
volumes: [pgdata]
```

### 5.3 CI/CD Pipeline (GitHub Actions)
1.  **Lint & Test:** Run ESLint, Angular Test, xUnit/.NET Test.
2.  **Build:** Build Docker images.
3.  **Scan:** Run Security Scan (Trivy/SonarQube).
4.  **Deploy:** Push to Registry → Deploy to K8s/App Service.
5.  **Migration:** Run `dotnet ef database update` automatically on deploy.

### 5.4 Environment Variables (Secrets)
- `JWT_SECRET` (32+ chars)
- `DB_PASSWORD` (Strong)
- `SMTP_HOST`, `SMTP_USER` (For notifications)
- `SENTRY_DSN` (For error tracking)
- `ENCRYPTION_KEY` (For PII)

---

## 6️⃣ AI CODING INSTRUCTIONS (CONSTRAINTS)
1.  **Strict Typing:** No `any`. Use Interfaces for DTOs, Models, and Store State.
2.  **Error Handling:** Never swallow errors. Log to Console + Sentry + NgRx Store.
3.  **Loading States:** Every async action must toggle `isLoading` in Store. Show Skeletons/Spinners.
4.  **Memory Management:** Unsubscribe all Observables (use `async` pipe or `takeUntilDestroyed`).
5.  **Code Style:** Follow Angular Style Guide & .NET Coding Conventions.
6.  **Comments:** Add XML comments in Backend, JSDoc in Frontend for public methods.
7.  **Testing:** Generate Unit Tests for every Service/Effect/Reducer created.
8.  **Verification:** After generating code, verify that the **Request Flow** (Section 3) is fully implemented in the code structure.

---

## 7️⃣ MISSING FEATURE DETECTION
*Before coding, ask yourself:*
- "Did I add Audit Logging to this endpoint?"
- "Did I normalize this data structure?"
- "Is there an SSE channel for this long-running task?"
- "Are the Policies checked in the Backend (not just Frontend)?"
- "Is the Deployment Config updated for this new service?"

*If the answer is NO, implement it before finishing the task.*
```

---

### 🛠️ How to Use This with Cursor/Copilot

1.  **Initialize Context:**
    *   Open Cursor/Copilot Chat.
    *   Type: `@PROJECT_SPEC.md` (or paste the content if not saved).
    *   Say: *"Read the PROJECT_SPEC.md. Scan the backend directory first. Report any missing normalization or SSE endpoints before we start coding."*

2.  **Feature Implementation:**
    *   Say: *"Implement the Employee Create Flow following Section 3.1 of the spec. Ensure Audit Logging is included in the backend service."*

3.  **SSE Implementation:**
    *   Say: *"Create the SSE service in Frontend and the corresponding Controller in Backend as per Section 2.3. Test the progress bar flow."*

4.  **Deployment:**
    *   Say: *"Generate the Dockerfile and docker-compose.yml based on Section 5. Ensure environment variables are secured."*

### 💡 Why This Prompt is Superior
1.  **Forces Backend Scan:** Prevents the AI from writing frontend code for a backend that doesn't exist or is poorly structured.
2.  **Normalization Enforcement:** Explicitly demands relational data modeling (Departments, Roles, Logs) instead of a simple CRUD app.
3.  **Granular Flow:** The **Request Flow** section ensures the AI doesn't skip interceptors, correlation IDs, or audit logs.
4.  **SSE Specifics:** Gives exact instructions on how to handle real-time streams (a common pain point).
5.  **DevOps Ready:** Includes Docker and CI/CD specs, making the project deployable immediately.
6.  **Self-Correction:** The "Missing Feature Detection" section forces the AI to double-check its own work against enterprise standards.
