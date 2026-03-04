# COMPREHENSIVE FRONTEND IMPLEMENTATION PLAN (ANGULAR VERSION)
## For EmployeeManagement System with JWT Authentication & Role-Based Access Control
---

## 2️⃣ FRONTEND ARCHITECTURE PLAN (ANGULAR)

### 2.1 Recommended Frontend Framework

**Angular 18+ with TypeScript**

**Justification:**
- ✅ Enterprise-grade framework built for complex applications
- ✅ Opinionated architecture (enforces best practices)
- ✅ Built-in dependency injection (cleaner than React)
- ✅ RxJS streams for reactive programming (perfect for auth token flows)
- ✅ Strong TypeScript integration (strict mode enabled)
- ✅ CLI tooling (code generation, schematics)
- ✅ Signals API (new reactive primitives in Angular 18+)
- ✅ Built-in HttpClient with interceptors
- ✅ Robust testing infrastructure (Jasmine, Karma)
- ✅ AOT compilation (better security, smaller bundles)
- ✅ Guard system (route protection at framework level)
- ✅ Resolver system (pre-fetch data before route navigation)
- ✅ Excellent for RBAC and complex state management

### 2.2 State Management Strategy

**Recommended: NgRx (with Signals) + RxJS Observables**

**Why NgRx over other options:**
- ✅ Redux pattern (predictable, time-travel debugging)
- ✅ Integration with Angular (official recommended)
- ✅ Entity adapter (built-in for CRUD operations)
- ✅ Selectors for memoized state derivation
- ✅ Effects for side-effects (API calls, logging)
- ✅ Router integration (route actions, state sync)
- ✅ DevTools extension support
- ✅ Reactive signals support (Angular 18+)

**Alternative (Lightweight):** Akita or simple RxJS BehaviorSubjects for smaller scope

**Architecture:**

```
┌──────────────────────────────────────────────────────┐
│          Angular Components                          │
│  (Templates with async pipe, event handlers)         │
└───────────────────┬──────────────────────────────────┘
                    │
        ┌───────────┴──────────┐
        │                      │
   ┌────▼─────┐          ┌────▼──────────┐
   │  NgRx    │          │   Services    │
   │  Store   │          │  (HTTP calls, │
   │  (Auth,  │          │   business    │
   │  Entities)          │   logic)      │
   └────┬─────┘          └────┬──────────┘
        │                     │
        ├─────────┬───────────┤
        │         │           │
   ┌────▼─────┬───▼────┐  ┌───▼──────────┐
   │ Reducers │ Effects │  │ HttpClient   │
   │ (state   │ (side   │  │ (interceptors)
   │  updates)│ effects)│  └───┬──────────┘
   └──────────┴────┬────┘      │
                   │           │
              ┌────▼───────────▼────┐
              │  .NET Backend API   │
              │  Endpoints          │
              └─────────────────────┘

Selectors:
  selectAuth$: Observable<AuthState>
  selectCurrentUser$: Observable<User>
  selectEmployees$: Observable<Employee[]>
  selectIsLoading$: Observable<boolean>
```

**NgRx Store Structure:**

```
Store (Root):
├── Auth Feature State
│   ├── currentUser: User | null
│   ├── accessToken: string | null
│   ├── accessTokenExpiresAt: number | null
│   ├── refreshTokenExpiryDate: Date | null
│   ├── isAuthenticated: boolean
│   ├── isLoading: boolean
│   └── error: string | null
│
├── Employees Feature State
│   ├── entities: { [id: string]: Employee }
│   ├── ids: string[]
│   ├── selectedId: string | null
│   ├── filters: EmployeeFilters
│   ├── pageNumber: number
│   ├── pageSize: number
│   ├── totalCount: number
│   ├── isLoading: boolean
│   └── error: string | null
│
└── UI Feature State
    ├── notifications: Notification[]
    ├── showDeleteModal: boolean
    ├── deleteTargetId: string | null
    ├── rateLimitReset: Date | null
    └── isRateLimited: boolean
```

**Key Patterns:**

**Selectors (Memoized State Derivation):**
```
selectAuthState$ → {currentUser, accessToken, isAuthenticated}
selectEmployeesList$ → Employees filtered/sorted/paginated
selectCanDeleteEmployee$ → Boolean (Admin + HR check)
selectRateLimitCountdown$ → Remaining seconds
```

**Effects (Side Effects Management):**
```
@Effect() login$ → Action → API Call → Success/Failure Action
@Effect() refreshToken$ → Triggered on token expiry → API Call → Update State
@Effect() logout$ → Clear State → Revoke Token → Redirect
@Effect() deleteEmployee$ → API Call → Invalidate Cache → Show Toast
```

**Error Handling in Effects:**
```
fetchEmployees$ → 401 → Dispatch RefreshToken Action → Retry
fetchEmployees$ → 403 → Dispatch ShowForbiddenError Action
fetchEmployees$ → 429 → Dispatch RateLimitTriggered Action
```

### 2.3 API Service Layer Structure

**Layered Architecture:**

```
src/
├── app/
│   ├── core/                           # Singleton services, guards, interceptors
│   │   ├── http/
│   │   │   ├── http.interceptor.ts     (Auth, CORS, Error handling)
│   │   │   ├── error.interceptor.ts    (401, 403, 429 handlers)
│   │   │   └── correlation-id.interceptor.ts
│   │   │
│   │   ├── services/
│   │   │   ├── auth.service.ts         (Login, Register, Logout, Token Refresh)
│   │   │   ├── employee.service.ts     (CRUD operations)
│   │   │   └── notification.service.ts (Toasts, alerts)
│   │   │
│   │   ├── guards/
│   │   │   ├── auth.guard.ts           (canActivate - check authenticated)
│   │   │   ├── admin.guard.ts          (canActivate - check Admin role)
│   │   │   ├── can-deactivate.guard.ts (canDeactivate - unsaved changes)
│   │   │   └── policy.guard.ts         (canActivate - composite policies)
│   │   │
│   │   ├── interceptors/               (HttpClient interceptors)
│   │   │   ├── auth.interceptor.ts
│   │   │   ├── error.interceptor.ts
│   │   │   └── logging.interceptor.ts
│   │   │
│   │   └── store/                      (NgRx root store)
│   │       ├── index.ts
│   │       └── root-store.module.ts
│   │
│   ├── features/                       # Feature modules
│   │   │
│   │   ├── auth/
│   │   │   ├── auth.module.ts
│   │   │   ├── store/
│   │   │   │   ├── auth.actions.ts
│   │   │   │   ├── auth.reducer.ts
│   │   │   │   ├── auth.effects.ts
│   │   │   │   ├── auth.selectors.ts
│   │   │   │   └── index.ts
│   │   │   │
│   │   │   ├── components/
│   │   │   │   ├── login/
│   │   │   │   ├── register/
│   │   │   │   └── logout-confirmation/
│   │   │   │
│   │   │   └── pages/
│   │   │       ├── login-page.ts
│   │   │       └── register-page.ts
│   │   │
│   │   ├── employees/
│   │   │   ├── employees.module.ts
│   │   │   ├── store/
│   │   │   │   ├── employee.actions.ts
│   │   │   │   ├── employee.reducer.ts
│   │   │   │   ├── employee.effects.ts
│   │   │   │   ├── employee.selectors.ts
│   │   │   │   └── index.ts
│   │   │   │
│   │   │   ├── components/
│   │   │   │   ├── employee-list/
│   │   │   │   ├── employee-detail/
│   │   │   │   ├── employee-form/
│   │   │   │   ├── employee-table/
│   │   │   │   ├── filter-bar/
│   │   │   │   └── delete-confirmation-dialog/
│   │   │   │
│   │   │   ├── pages/
│   │   │   │   ├── employee-list-page.ts
│   │   │   │   ├── employee-detail-page.ts
│   │   │   │   ├── employee-create-page.ts
│   │   │   │   └── employee-edit-page.ts
│   │   │   │
│   │   │   ├── resolvers/
│   │   │   │   ├── employee-detail.resolver.ts (load before navigation)
│   │   │   │   └── employees-list.resolver.ts
│   │   │   │
│   │   │   └── models/
│   │   │       ├── employee.model.ts
│   │   │       ├── employee-filter.model.ts
│   │   │       └── pagination.model.ts
│   │   │
│   │   ├── dashboard/
│   │   │   ├── dashboard.module.ts
│   │   │   ├── pages/
│   │   │   │   └── dashboard-page.ts
│   │   │   │
│   │   │   └── components/
│   │   │       ├── stat-card/
│   │   │       ├── employee-chart/
│   │   │       └── recent-activity/
│   │   │
│   │   ├── admin/
│   │   │   ├── admin.module.ts
│   │   │   ├── pages/
│   │   │   │   ├── admin-panel-page.ts
│   │   │   │   └── settings-page.ts
│   │   │   │
│   │   │   └── components/
│   │   │       ├── admin-users-table/
│   │   │       ├── system-stats/
│   │   │       └── recent-activity-log/
│   │   │
│   │   └── profile/
│   │       ├── profile.module.ts
│   │       ├── pages/
│   │       │   └── profile-page.ts
│   │       │
│   │       └── components/
│   │           └── profile-form/
│   │
│   ├── shared/                         # Shared across features
│   │   ├── components/
│   │   │   ├── layout/
│   │   │   │   ├── header.ts
│   │   │   │   ├── sidebar.ts
│   │   │   │   ├── main-layout.ts
│   │   │   │   └── breadcrumb.ts
│   │   │   │
│   │   │   ├── forms/
│   │   │   │   ├── form-field.ts
│   │   │   │   ├── text-input.ts
│   │   │   │   ├── select-field.ts
│   │   │   │   └── form-error.ts
│   │   │   │
│   │   │   ├── tables/
│   │   │   │   ├── data-table.ts
│   │   │   │   ├── table-header.ts
│   │   │   │   └── table-pagination.ts
│   │   │   │
│   │   │   ├── notifications/
│   │   │   │   ├── toast.ts
│   │   │   │   ├── toast-container.ts
│   │   │   │   ├── snackbar.ts
│   │   │   │   └── dialog.ts
│   │   │   │
│   │   │   ├── loaders/
│   │   │   │   ├── spinner.ts
│   │   │   │   ├── skeleton.ts
│   │   │   │   └── progress-bar.ts
│   │   │   │
│   │   │   └── empty-state/
│   │   │       └── empty-state.ts
│   │   │
│   │   ├── directives/
│   │   │   ├── has-role.directive.ts    (Show/hide by role)
│   │   │   ├── has-permission.directive.ts (Show/hide by policy)
│   │   │   └── loading-overlay.directive.ts
│   │   │
│   │   ├── pipes/
│   │   │   ├── format-date.pipe.ts
│   │   │   ├── format-currency.pipe.ts
│   │   │   ├── safe-html.pipe.ts
│   │   │   └── truncate.pipe.ts
│   │   │
│   │   ├── services/
│   │   │   ├── notification.service.ts  (Toast management)
│   │   │   └── dialog.service.ts        (Modal management)
│   │   │
│   │   ├── models/
│   │   │   ├── api-response.model.ts
│   │   │   ├── pagination.model.ts
│   │   │   └── notification.model.ts
│   │   │
│   │   └── shared.module.ts
│   │
│   ├── app.component.ts                # Root component
│   ├── app-routing.module.ts           # Main routing
│   └── app.module.ts                   # Root module
│
├── assets/
│   ├── images/
│   ├── icons/
│   └── styles/
│
├── environments/
│   ├── environment.ts
│   ├── environment.prod.ts
│   └── environment.staging.ts
│
├── main.ts
├── styles.scss
└── index.html
```

**Module Strategy (Lazy Loading):**

```
AppModule (Root)
├── CoreModule (provided once)
│   ├── HttpInterceptors
│   ├── Guards
│   └── Store configuration
│
├── SharedModule (imported by features)
│   ├── Common components
│   ├── Common pipes
│   └── Common directives
│
└── Feature Modules (lazy-loaded by routing)
    ├── AuthModule (forRoot())
    ├── EmployeesModule
    ├── DashboardModule
    ├── AdminModule
    └── ProfileModule
```

**HTTP Client Setup (Angular HttpClient):**

```
HttpClientModule with interceptors:

Request Interceptor Chain:
  1. CorsInterceptor (credentials: include)
  2. AuthInterceptor (add Authorization header)
  3. CorrelationIdInterceptor (add X-Correlation-ID)
  4. IdempotencyInterceptor (add Idempotency-Key)
  5. LoggingInterceptor (log requests)

Response Interceptor Chain:
  1. ErrorInterceptor (handle 401, 403, 429, 5xx)
  2. LoggingInterceptor (log responses)
  3. UnwrapApiResponseInterceptor (unwrap ApiResponse wrapper)
```

### 2.4 Authentication Handling Architecture (Angular)

**Multi-Layer Auth System:**

**Layer 1: HttpClient Interceptors**
```
AuthInterceptor (provided in HTTP_INTERCEPTORS):
  - Intercepts all HttpClient requests
  - Extracts access token from NgRx store
  - Adds Authorization header: Bearer {token}
  - Observable-based for reactive auth state

ErrorInterceptor:
  - Catches 401 responses
  - Triggers token refresh flow
  - Queues failing request
  - Retries after refresh (once)
  - Falls back to logout if refresh fails
```

**Layer 2: NgRx Effects**
```
AuthEffects:
  - @Effect() login$: Login action → HTTP call → Store user & token
  - @Effect() register$: Register action → HTTP call → Auto-login
  - @Effect() logout$: Logout action → Clear state → Navigate
  - @Effect() refreshToken$: RefreshToken action → HTTP call → Update token
  - @Effect() handleAuthError$: AuthError action → Redirect to login
```

**Layer 3: Service Layer**
```
AuthService:
  - Methods: login(), register(), logout(), refreshToken()
  - Returns: Observable<AuthResponse>
  - Uses HttpClient internally
  - Dispatches NgRx actions for state management
```

**Layer 4: Guards (Route Protection)**
```
AuthGuard (implements CanActivate):
  - Check: selectIsAuthenticated$ from store
  - If false: Navigate to /login
  - If true: Allow navigation

AdminGuard (implements CanActivate):
  - Check: selectIsAdmin$ (role === 'Admin')
  - If false: Navigate to /dashboard
  - If true: Allow navigation

PolicyGuard (implements CanActivate):
  - Check: selectCanDeleteEmployee$ (Admin + HR)
  - If false: Show error toast, prevent navigation
  - If true: Allow navigation

CanDeactivateGuard (implements CanDeactivate):
  - Check: Form has unsaved changes
  - If yes: Show confirmation dialog
  - User confirms/cancels
```

**Layer 5: Route Resolvers**
```
EmployeeDetailResolver (implements Resolve):
  - Pre-fetch employee data before navigation
  - If 404: Redirect back with error
  - If 403: Prevent navigation
  - Component receives resolved data via ActivatedRoute
```

**Token Refresh Strategy (Reactive with RxJS):**

```
Silent Refresh Approach:
  1. On app initialization, check token expiry
  2. If expiring soon (< 5 min): Dispatch RefreshTokenAction
  3. RefreshTokenEffect catches action
  4. Makes POST /refresh-token call
  5. Updates store with new access token
  6. Timer resets for next refresh window
  7. All subsequent requests use new token (interceptor reads from store)

Reactive Refresh Approach:
  1. Request fails with 401
  2. ErrorInterceptor dispatches RefreshTokenAction
  3. Same flow as above
  4. ErrorInterceptor queues original request
  5. After refresh completes, retry original request
  6. Return response to component
```

**Subscription Management:**

```
Components use async pipe (automatic unsubscribe):
  <div>{{currentUser$ | async}}</div>
  
OR manual OnDestroy:
  - Use takeUntil pattern
  - Unsubscribe on component destroy
  
Store selectors return Observables:
  currentUser$ = this.store.select(selectCurrentUser);
  isLoading$ = this.store.select(selectIsLoading);
```

### 2.5 Protected Routes Strategy (Angular)

**Route Guards Architecture:**

```
Guard Chain:
  Route → CanActivate (AuthGuard) → CanActivate (AdminGuard) → Load Component

Routes Configuration:
{
  path: '/login',
  component: LoginComponent
  // No guard - public
}

{
  path: '/dashboard',
  component: DashboardComponent,
  canActivate: [AuthGuard]
  // Requires authentication
}

{
  path: '/employees',
  loadChildren: 'features/employees/employees.module'
  canActivate: [AuthGuard],
  canDeactivate: [CanDeactivateGuard]
  children: [
    {
      path: '',
      component: EmployeeListComponent
    },
    {
      path: ':id',
      component: EmployeeDetailComponent,
      resolve: { employee: EmployeeDetailResolver }
    },
    {
      path: 'new',
      component: EmployeeCreateComponent,
      canActivate: [AdminGuard]
    },
    {
      path: ':id/edit',
      component: EmployeeEditComponent,
      canActivate: [AdminGuard, PolicyGuard]
    }
  ]
}

{
  path: '/admins',
  component: AdminPanelComponent,
  canActivate: [AuthGuard, AdminGuard]
}

{
  path: '**',
  component: NotFoundComponent
}
```

**Guard Implementation Pattern:**

```
export class AuthGuard implements CanActivate {
  constructor(
    private store: Store<AppState>,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.store.select(selectIsAuthenticated).pipe(
      take(1),
      map(isAuth => {
        if (!isAuth) {
          this.router.navigate(['/login']);
          return false;
        }
        return true;
      })
    );
  }
}
```

**Resolver Pattern:**

```
export class EmployeeDetailResolver implements Resolve<Employee> {
  constructor(
    private employeeService: EmployeeService,
    private router: Router
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Employee> {
    const id = route.paramMap.get('id');
    
    return this.employeeService.getEmployeeById(id).pipe(
      catchError(() => {
        this.router.navigate(['/employees']);
        return of(null);
      })
    );
  }
}

// In component:
constructor(private route: ActivatedRoute) {}

ngOnInit() {
  this.employee = this.route.snapshot.data['employee'];
}
```

### 2.6 Role-Based UI Rendering (Angular)

**Directive-Based Approach:**

```
<div *appHasRole="'Admin'">
  <button (click)="createEmployee()">Create Employee</button>
</div>

<div *appHasPermission="'CanDeleteEmployee'">
  <button (click)="deleteEmployee()">Delete</button>
</div>

Implementation pattern:
@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective {
  constructor(
    private store: Store<AppState>,
    private container: ViewContainerRef,
    private template: TemplateRef<any>
  ) {}

  @Input()
  set appHasRole(role: string) {
    this.store.select(selectCurrentUser).pipe(
      take(1),
      map(user => user?.role === role)
    ).subscribe(hasRole => {
      if (hasRole) {
        this.container.createEmbeddedView(this.template);
      } else {
        this.container.clear();
      }
    });
  }
}
```

**Component-Based Approach (for complex logic):**

```
<app-action-buttons
  [employee]="employee$ | async"
  [currentUser]="currentUser$ | async"
  (edit)="onEdit($event)"
  (delete)="onDelete($event)">
</app-action-buttons>

Component handles visibility logic:
  - Receives employee and currentUser as inputs
  - Outputs edit/delete events
  - Parent decides which buttons to show
```

**Using Async Pipe with Store Selectors:**

```
<!-- Role-aware navigation menu -->
<nav>
  <a routerLink="/dashboard">Dashboard</a>
  <a routerLink="/employees">Employees</a>
  
  <!-- Only show if Admin -->
  <a *ngIf="isAdmin$ | async" routerLink="/employees/new">
    Create Employee
  </a>
  
  <!-- Only show if Admin -->
  <a *ngIf="isAdmin$ | async" routerLink="/admins">
    Admin Panel
  </a>
</nav>

In component:
isAdmin$ = this.store.select(selectIsAdmin);
```

### 2.7 Error Handling System (Angular)

**Global Error Interceptor Strategy:**

```
HttpErrorResponse → ErrorInterceptor → Categorize → Dispatch NgRx Action
                                                   ↓
                                           Store state updated
                                                   ↓
                                    Component reads state
                                                   ↓
                                    Component displays error
```

**Error Categories:**

**400 Bad Request (Validation)**
```
Interceptor:
  - Extract field errors from response.data
  - Dispatch AddFormErrors action
  
Store:
  - formErrors: { field: string, error: string }[]
  
Component:
  - Read formErrors$ from store
  - Show inline errors below fields
  - Example: email already registered
```

**401 Unauthorized (Token Invalid)**
```
Interceptor:
  - Check if request is to /refresh-token
  - If yes: Dispatch Logout action (refresh failed)
  - If no: Dispatch RefreshToken action (attempt refresh)
           Queue original request
  
After Refresh:
  - If success: Update token, retry original request
  - If fail: Dispatch Logout action
  
Store:
  - isAuthenticated: false
  - error: 'Session expired'
  
Component:
  - Redirect to /login
  - Show toast: 'Session expired. Please log in again.'
```

**403 Forbidden (Permission Denied)**
```
Interceptor:
  - Dispatch ShowPermissionError action
  - Do NOT retry, do NOT logout
  
Store:
  - error: 'You don\'t have permission...'
  
Component:
  - Show toast: error message
  - Disable action button
  - NO navigation change (stay on current page)
```

**429 Too Many Requests (Rate Limited)**
```
Interceptor:
  - Extract Retry-After header
  - Dispatch RateLimitTriggered action with delaySeconds
  
Store:
  - rateLimitReset: Date (now + delaySeconds)
  - isRateLimited: true
  
Component:
  - Async pipe reads rateLimitReset$
  - Calculate countdown timer
  - Disable submit button until reset
  - Show: 'Retry in {X} seconds'
  
OR Auto-retry:
  - Queue request
  - After delaySeconds: Retry automatically
```

**5xx Server Error**
```
Interceptor:
  - Extract correlationId
  - Dispatch ShowServerError action
  - Log to error tracking service (Sentry, etc.)
  
Store:
  - error: 'Something went wrong. Try again later.'
  - correlationId: from response
  
Component:
  - Show toast with correlation ID
  - 'Error ID: {correlationId}'
  - Log correlation ID for support
```

**Error Handler Service Pattern:**

```
export class ErrorHandlerService {
  handleError(error: HttpErrorResponse): Observable<never> {
    const errorResponse = {
      status: error.status,
      message: this.getErrorMessage(error),
      correlationId: error.error?.correlationId
    };

    // Log error
    this.logError(errorResponse);

    // Dispatch appropriate action based on status
    switch (error.status) {
      case 401:
        this.store.dispatch(new RefreshTokenAction());
        break;
      case 403:
        this.store.dispatch(new ShowPermissionErrorAction());
        break;
      case 429:
        this.store.dispatch(new RateLimitTriggeredAction());
        break;
      case 400:
        const fieldErrors = error.error?.data;
        this.store.dispatch(new AddFormErrorsAction(fieldErrors));
        break;
      case 500:
      default:
        this.store.dispatch(new ShowServerErrorAction(errorResponse));
    }

    return throwError(() => errorResponse);
  }
}
```

### 2.8 Global Notification Strategy (Angular)

**Toast/Snackbar System:**

```
Angular Material MatSnackBar OR Custom Toast Service

NotificationService:
  - success(message: string, duration?: number)
  - error(message: string)
  - warning(message: string)
  - info(message: string)
  
Implementation:
  - Inject NotificationService in components
  - Call service method on action success/failure
  - Snackbar auto-dismisses after 5 seconds
  - User can manually dismiss

Usage in Effects:
  ofType(DeleteEmployeeSuccess).pipe(
    tap(() => this.notificationService.success('Employee deleted'))
  )
```

**Dialog (Modal) System:**

```
Angular Material MatDialog OR Custom Dialog Component

DialogService:
  - confirm(title, message): Observable<boolean>
  - alert(title, message): Observable<void>
  - custom(component, data): Observable<any>

Usage:
  deleteEmployee() {
    this.dialogService.confirm(
      'Delete Employee?',
      'This cannot be undone.'
    ).pipe(
      filter(result => result), // Only if confirmed
      switchMap(() => this.employeeService.delete(id))
    ).subscribe(...)
  }
```

**In-Form Validation Errors:**

```
Component reads formErrors$ from store:
  
formErrors$: Observable<FormError[]> = 
  this.store.select(selectFormErrors);

Template:
  <app-text-input 
    formControlName="email"
    [error]="(formErrors$ | async)?.find(e => e.field === 'email')?.error">
  </app-text-input>
```

**Page-Level Alerts:**

```
<div *ngIf="error$ | async as error" class="alert alert-error">
  {{error.message}}
  <small *ngIf="error.correlationId">
    Error ID: {{error.correlationId}}
  </small>
</div>
```

### 2.9 Form Validation Architecture (Angular)

**Reactive Forms with Validators:**

```
Pattern:
  - FormBuilder for form creation
  - Custom validators (async for email check)
  - Real-time validation (valueChanges observable)
  - Server-side error integration

Implementation:

export class LoginFormComponent implements OnInit {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private store: Store
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    
    this.store.dispatch(
      new LoginAction(this.form.value)
    );
  }

  get emailError$() {
    return this.form.get('email')?.statusChanges.pipe(
      map(() => {
        const control = this.form.get('email');
        if (control?.hasError('required')) return 'Email required';
        if (control?.hasError('email')) return 'Invalid email';
        return null;
      })
    );
  }
}
```

**Async Validators (Email Uniqueness):**

```
export class UniqueEmailValidator implements AsyncValidator {
  constructor(private authService: AuthService) {}

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    if (!control.value) return of(null);

    return this.authService.checkEmailExists(control.value).pipe(
      map(exists => exists ? { emailExists: true } : null),
      debounceTime(300)
    );
  }
}

// In form:
email: ['', [Validators.required], [this.uniqueEmailValidator]]
```

**Server-Side Error Integration:**

```
From Effects:
  AuthEffects.login$ → 400 error → Extract validation errors → 
  Dispatch AddFormErrorsAction

In Component:
  formErrors$ = this.store.select(selectFormErrors);
  
  Display errors:
    <span *ngIf="(formErrors$ | async)?.email">
      {{(formErrors$ | async)?.email}}
    </span>
```

### 2.10 Retry & Backoff Strategy (Angular)

**RxJS Retry Operators:**

```
Pattern: retryWhen + exponential backoff

Source Observable → retryWhen(errors => {
  errors.pipe(
    tap(error => checkIf429OrTemporary),
    scan((errorCount, error) => errorCount + 1, 0),
    switchMap(errorCount => {
      const delayMs = Math.min(
        initialDelay * Math.pow(2, errorCount - 1),
        maxDelay
      );
      return timer(delayMs);
    })
  )
})

Usage:
export class EmployeeService {
  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>('/api/employees').pipe(
      retryWhen(errors =>
        errors.pipe(
          mergeMap((error, index) => {
            // Don't retry 401, 403, 404
            if ([401, 403, 404].includes(error.status)) {
              return throwError(() => error);
            }

            // Retry 429, 5xx with exponential backoff
            const delayMs = Math.min(1000 * Math.pow(2, index), 60000);
            return timer(delayMs);
          }),
          take(7) // Max 7 retries
        )
      )
    );
  }
}
```

**Rate Limit Queuing (for 429):**

```
Pattern: Use a queue service that debounces and throttles

RequestQueueService:
  - Maintains queue of pending requests
  - When 429: Stops processing queue
  - Sets timer for Retry-After
  - Resumes queue after delay

Usage in Interceptor:
  if (error.status === 429) {
    const retryAfter = error.headers.get('Retry-After');
    this.requestQueue.pause(parseInt(retryAfter) * 1000);
    
    return this.requestQueue.queue(() => {
      return this.http.request(req);
    });
  }
```

**UI Feedback:**

```
Component monitors NgRx state:

isRateLimited$ = this.store.select(selectIsRateLimited);
retryCountdown$ = this.store.select(selectRetryCountdown).pipe(
  interval(1000),
  map(elapsed => retryAfterSeconds - elapsed)
);

Template:
<button [disabled]="isRateLimited$ | async">
  {{(retryCountdown$ | async) ? 'Retry in ' + (retryCountdown$ | async) + 's' : 'Submit'}}
</button>
```

---

## 3️⃣ FOLDER & MODULE STRUCTURE PLAN

*(Already provided above in Section 2.3 with Angular conventions)*

**Key Differences from React:**
- Feature modules with separate routing
- CoreModule for singletons
- SharedModule for reusables
- Store per feature (not global)
- Services injected via constructor (DI)
- Guards and Resolvers at route level
- Pipes for reusable transformations
- Directives for DOM manipulation

---

## 4️⃣ UI/UX PLANNING

*(Same as previous - Angular renders the same UI/Pages/Layouts)*

---

## 5️⃣ SECURITY STRATEGY

### 5.1 JWT Storage (Angular)

**Access Token:**
```
Stored in: NgRx Store (in-memory)
  - Not in localStorage
  - Not in sessionStorage
  - Lost on page refresh
  - Protected from XSS

Injection Point:
  - AuthInterceptor reads token from store
  - Adds to Authorization header
  - HttpClient requests include token
  
Page Refresh Flow:
  1. Page reloads, store reset
  2. App initialization
  3. Check refresh token from cookie
  4. Dispatch RefreshTokenAction
  5. Fetch new access token
  6. Store in NgRx
  7. Page content loads with valid token
```

**Refresh Token:**
```
Stored in: HttpOnly Secure SameSite=Strict Cookie
  - Not accessible to JS
  - Auto-sent by browser with each request
  - Protected from XSS and CSRF
  
Never in JavaScript:
  - Not stored in localStorage
  - Not stored in NgRx
  - Not sent in Authorization header
  
Refresh Flow:
  1. ErrorInterceptor catches 401
  2. AuthService calls POST /refresh-token
  3. Server validates refresh token from cookie
  4. Server issues new refreshToken cookie
  5. Browser automatically updates cookie
  6. Access token returned in response
  7. Access token stored in NgRx
```

### 5.2 Refresh Token Workflow (Angular)

**Token Refresh with Effects:**

```
RefreshTokenEffect:
  1. Listens for RefreshTokenAction
  2. Calls authService.refreshToken()
  3. On success:
     - Dispatch UpdateTokenAction
     - Update store with new token
     - Return success action
  4. On error:
     - Dispatch LogoutAction (refresh failed)
     - Clear state

TokenExpiryEffect:
  1. Listen to token issued
  2. Calculate expiry time
  3. Set timer for (expiryTime - 5 minutes)
  4. When timer fires: Dispatch RefreshTokenAction
  5. Proactive refresh without user awareness
```

**Max 1 Refresh per Request:**

```
State management:
  isRefreshing$: Observable<boolean> in store

ErrorInterceptor logic:
  if (error.status === 401 && !isRefreshing) {
    1. Dispatch RefreshTokenAction
    2. Subscribe to store.select(selectIsRefreshing)
    3. When isRefreshing becomes false:
       - Retry original request
       - Return result
  } else if (isRefreshing) {
    // Already refreshing, wait for it
    return store.select(selectIsRefreshing).pipe(
      filter(refreshing => !refreshing),
      take(1),
      switchMap(() => this.http.request(req))
    );
  }
```

### 5.3 XSS/CSRF Protections (Angular)

**XSS Prevention:**

1. **Angular's Built-in Sanitization:**
   - Angular sanitizes all string interpolation by default
   - `{{userInput}}` is safe (text-only)
   - Unsafe HTML requires explicit declaration

2. **Safe HTML Pipe (for trusted content):**
```
<div [innerHTML]="content | safeHtml"></div>
   
   Pipe implementation:
     DomSanitizer.sanitize(SecurityContext.HTML, value)
```

3. **Content Security Policy (CSP):**
```
In index.html meta or server headers:
   default-src 'self'
   script-src 'self'
   style-src 'self' 'unsafe-inline'
   img-src 'self' data: https:
   connect-src 'self' https://api.example.com
```

4. **Avoid Inline Scripts:**
   - No onclick handlers in templates
   - No eval() or Function()
   - Use event binding: (click)="handler()"

**CSRF Prevention:**

1. **SameSite Cookie:**
   - Backend sets: SameSite=Strict on refresh token
   - Browser only sends to same origin
   - Prevents cross-site requests

2. **CORS Configuration:**
   - Backend allows only your domain
   - Frontend respects CORS headers
   - No credentials in cross-origin requests

3. **CSRF Token (Optional):**
   - For extra protection, backend can require X-CSRF-Token
   - HttpClient interceptor adds it to requests
   - Server validates before processing

### 5.4 Handling 401, 403, 429 Globally (Angular)

**Error Interceptor Strategy:**

```
export class ErrorInterceptor implements HttpInterceptor {
  
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        
        if (error.status === 401) {
          // Token invalid/expired
          return this.handle401(error, req, next);
        }
        
        if (error.status === 403) {
          // Permission denied
          return this.handle403(error);
        }
        
        if (error.status === 429) {
          // Rate limited
          return this.handle429(error);
        }
        
        // Other errors
        return this.handleOtherErrors(error);
      })
    );
  }

  private handle401(
    error: HttpErrorResponse,
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    // Check if this is a 401 from refresh endpoint itself
    if (req.url.includes('/refresh-token')) {
      // Refresh failed - must logout
      this.store.dispatch(new LogoutAction());
      this.router.navigate(['/login']);
      return throwError(() => error);
    }

    // Normal endpoint got 401 - try to refresh
    return this.store.select(selectIsRefreshing).pipe(
      take(1),
      switchMap(isRefreshing => {
        
        if (isRefreshing) {
          // Already refreshing, wait for it
          return this.store.select(selectIsRefreshing).pipe(
            skipWhile(r => r === true),
            take(1),
            switchMap(() => next.handle(req))
          );
        }

        // Start refresh
        this.store.dispatch(new RefreshTokenAction());
        
        // Wait for refresh to complete
        return this.store.select(selectIsRefreshing).pipe(
          skipWhile(r => r === true),
          take(1),
          switchMap(() => {
            // Refresh complete - add new token and retry
            const token = this.store.selectSnapshot(selectAccessToken);
            const authReq = req.clone({
              setHeaders: { Authorization: `Bearer ${token}` }
            });
            return next.handle(authReq);
          }),
          catchError(() => {
            // Refresh failed
            this.store.dispatch(new LogoutAction());
            return throwError(() => error);
          })
        );
      })
    );
  }

  private handle403(error: HttpErrorResponse): Observable<never> {
    this.notificationService.error(
      'You don\'t have permission to perform this action.'
    );
    return throwError(() => error);
  }

  private handle429(error: HttpErrorResponse): Observable<never> {
    const retryAfter = error.headers.get('Retry-After');
    const delaySeconds = parseInt(retryAfter) || 60;

    this.store.dispatch(
      new RateLimitTriggeredAction(delaySeconds)
    );

    this.notificationService.warning(
      `Too many requests. Try again in ${delaySeconds} seconds.`
    );

    return throwError(() => error);
  }
}
```

---

## 6️⃣ API INTEGRATION PLAN

### 6.1 Consuming ApiResponse Wrapper (Angular)

**Response Unwrapping:**

```
export class UnwrapApiResponseInterceptor implements HttpInterceptor {
  
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    return next.handle(req).pipe(
      map(event => {
        if (event instanceof HttpResponse) {
          const apiResponse: ApiResponse<any> = event.body;
          
          // Unwrap the data portion
          return event.clone({
            body: apiResponse.data,
            // Keep correlation ID in response for access
            headers: event.headers.set(
              'X-Correlation-ID',
              apiResponse.correlationId || ''
            )
          });
        }
        return event;
      })
    );
  }
}

// Alternative: In Service layer
getEmployees(filters: EmployeeFilters): Observable<EmployeeListResponse> {
  return this.http.get<ApiResponse<EmployeeListResponse>>(
    '/api/employees',
    { params: filters as any }
  ).pipe(
    map(response => {
      this.correlationId = response.correlationId;
      return response.data; // Extract data
    })
  );
}
```

**Type Safety:**

```
// Models
export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  correlationId: string;
}

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  department: string;
  role: string;
  isActive: boolean;
}

export interface EmployeeListResponse {
  items: Employee[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Service
getEmployees(): Observable<EmployeeListResponse> {
  return this.http.get<ApiResponse<EmployeeListResponse>>(...);
}
```

### 6.2 Correlation ID Handling (Angular)

**Generation & Transmission:**

```
export class CorrelationIdInterceptor implements HttpInterceptor {
  
  constructor(
    private store: Store<AppState>
  ) {}

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    // Get or generate correlation ID
    let correlationId = this.store.selectSnapshot(selectCorrelationId);
    
    if (!correlationId) {
      correlationId = this.generateId();
      this.store.dispatch(new SetCorrelationIdAction(correlationId));
    }

    // Add to request
    const authReq = req.clone({
      setHeaders: { 'X-Correlation-ID': correlationId }
    });

    return next.handle(authReq);
  }

  private generateId(): string {
    return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
}
```

**Tracking in Errors:**

```
// ErrorHandlerService
handleApiError(error: HttpErrorResponse): void {
  const correlationId = error.headers.get('X-Correlation-ID') ||
                       error.error?.correlationId;

  const errorData = {
    status: error.status,
    message: error.error?.message,
    correlationId: correlationId,
    timestamp: new Date().toISOString(),
    url: error.url
  };

  // Log to error tracking
  this.errorTrackingService.logError(errorData);

  // Store in state for display
  this.store.dispatch(new SetErrorAction(errorData));
}

// Component displays it
<div *ngIf="error$ | async as error">
  {{error.message}}
  <small *ngIf="error.correlationId">
    Error ID: {{error.correlationId}}
  </small>
</div>
```

### 6.3 Rate Limit Headers (Angular)

**Header Extraction:**

```
export class RateLimitInterceptor implements HttpInterceptor {
  
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    
    return next.handle(req).pipe(
      tap(event => {
        if (event instanceof HttpResponse) {
          // Check for rate limit warnings
          const limit = event.headers.get('X-RateLimit-Limit');
          const remaining = event.headers.get('X-RateLimit-Remaining');
          const reset = event.headers.get('X-RateLimit-Reset');

          if (remaining && parseInt(remaining) < 10) {
            this.store.dispatch(
              new RateLimitWarningAction({
                remaining: parseInt(remaining),
                limit: parseInt(limit),
                resetAt: new Date(parseInt(reset) * 1000)
              })
            );
          }
        }
      })
    );
  }
}

// On 429 error
if (error.status === 429) {
  const retryAfter = error.headers.get('Retry-After');
  const delaySeconds = parseInt(retryAfter) || 60;

  this.store.dispatch(
    new RateLimitTriggeredAction(delaySeconds)
  );
}
```

**UI Countdown:**

```
// Selector
selectRateLimitCountdown$ = this.store.select(selectRateLimitReset).pipe(
  switchMap(resetAt => {
    if (!resetAt) return of(0);
    
    return interval(1000).pipe(
      map(() => Math.max(0, Math.ceil((resetAt.getTime() - Date.now()) / 1000)))
    );
  })
);

// Template
<button [disabled]="(rateLimitCountdown$ | async) > 0">
  {{(rateLimitCountdown$ | async) > 0 
    ? 'Retry in ' + (rateLimitCountdown$ | async) + 's'
    : 'Submit'}}
</button>
```

### 6.4 Interceptor Strategy (Angular)

**Complete Interceptor Chain:**

```
Provided in AppModule:

HTTP_INTERCEPTORS tokens (executed in order):
  1. CorrelationIdInterceptor (add X-Correlation-ID)
  2. AuthInterceptor (add Authorization header)
  3. IdempotencyInterceptor (add Idempotency-Key)
  4. RateLimitInterceptor (monitor rate limit headers)
  5. UnwrapApiResponseInterceptor (unwrap ApiResponse wrapper)
  6. ErrorInterceptor (handle errors - 401, 403, 429, 5xx)
  7. LoggingInterceptor (log requests/responses)

Request Flow (order matters):
  1. Correlation ID added
  2. Auth token added
  3. Idempotency key added
  4. Send to server

Response Flow (reverse order):
  1. Logging interceptor records response
  2. Error interceptor checks for errors (catches 401, 403, 429)
  3. Unwrap API response wrapper
  4. Monitor rate limit headers
  5. Return to caller
```

---

## 7️⃣ PERFORMANCE STRATEGY

### 7.1 Code Splitting (Angular)

**Lazy Loading Modules:**

```
Routes configuration:
{
  path: 'employees',
  loadChildren: () => import('./features/employees/employees.module')
    .then(m => m.EmployeesModule)
  // Module loaded only when route accessed
}

{
  path: 'admin',
  loadChildren: () => import('./features/admin/admin.module')
    .then(m => m.AdminModule)
}

Result:
  - main.bundle.js (core only)
  - employees.module.chunk.js (loaded on demand)
  - admin.module.chunk.js (loaded on demand)
```

**Preloading Strategy:**

```
export class SelectivePreloadingStrategy implements PreloadingStrategy {
  preload(route: Route, fn: () => Observable<any>): Observable<any> {
    // Only preload if route has preload: true
    return route.data && route.data['preload'] ? fn() : of(null);
  }
}

Routes:
{
  path: 'employees',
  loadChildren: 'features/employees/employees.module',
  data: { preload: true } // Preload this module after initial load
}
```

### 7.2 Lazy Loading (Angular)

**Component Lazy Loading:**

```
Example: Modal component loaded only when opened

In lazy-loaded module:
export const EMPLOYEE_ROUTES: Routes = [
  {
    path: '',
    component: EmployeeListComponent
  },
  {
    path: 'delete-dialog',
    component: DeleteConfirmationComponent
    // Only loaded when route activated
  }
];

Or with @defer (Angular 18+):
@defer (when isDeleteModalOpen$ | async) {
  <app-delete-confirmation-dialog />
}
```

**Image Lazy Loading:**

```
Template:
<img [src]="imageSrc" loading="lazy" alt="..." />

Or with directive:
<img appLazyLoad [src]="imageSrc" alt="..." />
```

### 7.3 Memoization (Angular)

**Using computed() from @angular/core (Angular 18+):**

```
export class EmployeeListComponent {
  employees = input.required<Employee[]>();
  filters = input.required<EmployeeFilters>();

  // Memoized computed value
  filteredEmployees = computed(() => {
    return this.employees().filter(emp => {
      if (filters().department && emp.department !== filters().department)
        return false;
      if (filters().isActive !== undefined && emp.isActive !== filters().isActive)
        return false;
      return true;
    });
  });

  // Only recalculates when inputs change
}
```

**Using Async Pipe (auto-memoization in change detection):**

```
<div>
  <!-- Only re-renders if observable emits new value -->
  {{employees$ | async | slice:0:10}}
</div>
```

**Using OnPush Change Detection:**

```
@Component({
  selector: 'app-employee-row',
  template: `<tr><td>{{employee.name}}</td></tr>`,
  changeDetection: ChangeDetectionStrategy.OnPush
  // Only checks when @Input changes or events fire
})
export class EmployeeRowComponent {
  @Input() employee: Employee;
}
```

### 7.4 Caching Strategy (Angular)

**HTTP Caching (via Interceptor):**

```
export class CachingInterceptor implements HttpInterceptor {
  private cache = new Map<string, CachedResponse>();

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    
    // Only cache GET requests
    if (req.method !== 'GET') {
      return next.handle(req);
    }

    const cachedResponse = this.cache.get(req.url);
    
    if (cachedResponse && !this.isStale(cachedResponse)) {
      return of(new HttpResponse({
        body: cachedResponse.body,
        status: 200,
        url: req.url
      }));
    }

    return next.handle(req).pipe(
      tap(event => {
        if (event instanceof HttpResponse) {
          this.cache.set(req.url, {
            body: event.body,
            timestamp: Date.now(),
            ttl: 5 * 60 * 1000 // 5 minutes
          });
        }
      })
    );
  }

  private isStale(cached: CachedResponse): boolean {
    return Date.now() - cached.timestamp > cached.ttl;
  }
}
```

**Store Caching (NgRx):**

```
// Reducer tracks loading state
const employeesFeature = createFeature({
  name: 'employees',
  reducer: createReducer(
    initialState,
    on(loadEmployees, (state) => ({...state, isLoading: true})),
    on(loadEmployeesSuccess, (state, {employees}) => ({
      ...state,
      ids: employees.map(e => e.id),
      entities: {
        ...state.entities,
        ...employees.reduce((acc, e) => ({...acc, [e.id]: e}), {})
      },
      isLoading: false,
      lastLoaded: Date.now()
    }))
  )
});

// Effect checks cache before fetching
loadEmployees$ = createEffect(() =>
  this.actions$.pipe(
    ofType(loadEmployees),
    concatLatestFrom(() => 
      this.store.select(selectLastEmployeesLoaded)
    ),
    switchMap(([action, lastLoaded]) => {
      // If loaded less than 5 minutes ago, skip
      if (lastLoaded && Date.now() - lastLoaded < 5 * 60 * 1000) {
        return of(loadEmployeesSuccess({employees: []})); // No-op
      }

      return this.employeeService.getEmployees().pipe(
        map(employees => loadEmployeesSuccess({employees})),
        catchError(error => of(loadEmployeesFailure({error})))
      );
    })
  )
);
```

**Session Storage for Temporary State:**

```
// Save filter state to session storage
saveFilterState(filters: EmployeeFilters): void {
  sessionStorage.setItem('employeeFilters', JSON.stringify(filters));
}

// Restore on page reload
restoreFilterState(): EmployeeFilters | null {
  const saved = sessionStorage.getItem('employeeFilters');
  return saved ? JSON.parse(saved) : null;
}

// Clear on logout
clearSessionStorage(): void {
  sessionStorage.removeItem('employeeFilters');
  sessionStorage.removeItem('lastEmployeeView');
}
```

### 7.5 Additional Performance Optimizations

**Virtual Scrolling (for large lists):**

```
<cdk-virtual-scroll-viewport itemSize="50" class="example-viewport">
  <div *cdkVirtualFor="let employee of employees$ | async" class="example-item">
    {{employee.name}}
  </div>
</cdk-virtual-scroll-viewport>

// Only renders visible items in DOM
// Smooth scrolling for 1000+ items
```

**Pagination (vs Infinite Scroll):**

```
- Load only current page (25-50 items)
- User controls when to load more
- Faster initial load
- Clear page boundaries
```

**OnPush Change Detection Strategy:**

```
Applied across all components:

@Component({
  selector: 'app-employee',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class EmployeeComponent {
  @Input() employee: Employee; // OnPush checks only on @Input change
  @Input() onEdit: Function;
}

Benefits:
  - Change detection only when @Input/@Output changes
  - Not on every parent re-render
  - Faster for large component trees
```

**Unsubscribe Pattern:**

```
Pattern 1: takeUntil
export class EmployeeListComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  ngOnInit() {
    this.employees$ = this.employeeService.getEmployees().pipe(
      takeUntil(this.destroy$)
    );
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

Pattern 2: Async pipe (auto-unsubscribe)
<div>{{employees$ | async}}</div>
// Angular unsubscribes when component destroys
```

---

## 8️⃣ TESTING STRATEGY

### 8.1 Unit Tests (Angular)

**Store/Reducer Tests:**

```
describe('AuthReducer', () => {
  it('should set access token on LoginSuccess', () => {
    const action = loginSuccess({
      accessToken: 'token123',
      user: mockUser
    });

    const state = authReducer(initialState, action);

    expect(state.accessToken).toBe('token123');
    expect(state.currentUser).toEqual(mockUser);
  });

  it('should clear state on Logout', () => {
    const state = authReducer(stateWithToken, logoutAction());
    
    expect(state.accessToken).toBeNull();
    expect(state.currentUser).toBeNull();
  });
});
```

**Effects Tests:**

```
describe('AuthEffects', () => {
  let actions$: Observable<Action>;
  let effects: AuthEffects;
  let authService: AuthService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthEffects,
        provideMockActions(() => actions$),
        {
          provide: AuthService,
          useValue: jasmine.createSpyObj('AuthService', ['login'])
        }
      ]
    });

    effects = TestBed.inject(AuthEffects);
    authService = TestBed.inject(AuthService);
  });

  it('should return loginSuccess on successful login', () => {
    const action = login({email: 'test@test.com', password: 'pass'});
    const response = {id: 1, accessToken: 'token'};
    const completion = loginSuccess(response);

    actions$ = hot('-a', {a: action});
    const response$ = cold('-|', {}, response);
    const expected = cold('--b', {b: completion});

    authService.login.and.returnValue(response$);

    expect(effects.login$).toBeObservable(expected);
  });
});
```

**Service Tests:**

```
describe('EmployeeService', () => {
  let service: EmployeeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EmployeeService]
    });

    service = TestBed.inject(EmployeeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  it('should fetch employees', () => {
    const mockEmployees = [{id: 1, firstName: 'John'}];

    service.getEmployees().subscribe(data => {
      expect(data.length).toBe(1);
      expect(data[0].firstName).toBe('John');
    });

    const req = httpMock.expectOne('/api/employees');
    expect(req.request.method).toBe('GET');
    req.flush({data: mockEmployees});
  });
});
```

**Directive Tests:**

```
describe('HasRoleDirective', () => {
  let component: TestComponent;
  let fixture: ComponentFixture<TestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [HasRoleDirective, TestComponent],
      providers: [MockStore]
    }).compileComponents();

    fixture = TestBed.createComponent(TestComponent);
    component = fixture.componentInstance;
  });

  it('should show element for matching role', () => {
    const store = TestBed.inject(Store);
    store.setState({auth: {currentUser: {role: 'Admin'}}});

    fixture.detectChanges();

    const element = fixture.debugElement.query(By.directive(HasRoleDirective));
    expect(element).toBeTruthy();
  });
});
```

### 8.2 Integration Tests (Angular)

**Component Integration Tests:**

```
describe('LoginComponent Integration', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let store: MockStore;
  let authService: AuthService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [LoginComponent],
      imports: [ReactiveFormsModule, HttpClientTestingModule],
      providers: [
        provideMockStore(),
        AuthService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    store = TestBed.inject(Store) as MockStore;
    authService = TestBed.inject(AuthService);
  });

  it('should dispatch login action on form submit', () => {
    const dispatchSpy = spyOn(store, 'dispatch');

    component.form.patchValue({
      email: 'test@test.com',
      password: 'password123'
    });

    component.onSubmit();

    expect(dispatchSpy).toHaveBeenCalledWith(
      jasmine.objectContaining({
        type: '[Auth] Login'
      })
    );
  });

  it('should display validation errors', async () => {
    const emailInput = fixture.debugElement.query(
      By.css('input[name="email"]')
    ).nativeElement;

    emailInput.value = 'invalid-email';
    emailInput.dispatchEvent(new Event('blur'));

    fixture.detectChanges();
    await fixture.whenStable();

    const errorMsg = fixture.debugElement.query(
      By.css('[data-testid="email-error"]')
    );

    expect(errorMsg.nativeElement.textContent).toContain('Invalid email');
  });

  it('should show loading state during login', () => {
    store.setState({auth: {isLoading: true}});

    fixture.detectChanges();

    const button = fixture.debugElement.query(
      By.css('button[type="submit"]')
    );

    expect(button.nativeElement.disabled).toBe(true);
    expect(button.nativeElement.textContent).toContain('Logging in...');
  });
});
```

**Error Handling Integration:**

```
describe('Error Interceptor Integration', () => {
  let httpClient: HttpClient;
  let httpMock: HttpTestingController;
  let store: MockStore;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ErrorInterceptor]
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
    store = TestBed.inject(Store) as MockStore;
  });

  it('should dispatch RefreshToken on 401', () => {
    const dispatchSpy = spyOn(store, 'dispatch');

    httpClient.get('/api/employees').subscribe();

    const req = httpMock.expectOne('/api/employees');
    req.flush({error: 'Unauthorized'}, {status: 401, statusText: 'Unauthorized'});

    expect(dispatchSpy).toHaveBeenCalledWith(
      jasmine.objectContaining({type: '[Auth] Refresh Token'})
    );
  });

  it('should show error on 429 rate limit', () => {
    const notificationService = TestBed.inject(NotificationService);
    const notifySpy = spyOn(notificationService, 'warning');

    httpClient.get('/api/employees').subscribe(
      () => {},
      () => {} // Catch error
    );

    const req = httpMock.expectOne('/api/employees');
    req.flush({}, {status: 429, statusText: 'Too Many Requests',
               headers: {'retry-after': '60'}});

    expect(notifySpy).toHaveBeenCalledWith(jasmine.stringMatching('Too many requests'));
  });
});
```

### 8.3 E2E Tests (Angular with Cypress/Playwright)

**Complete User Journeys:**

```
describe('Login and CRUD Flow', () => {
  
  it('should complete full employee lifecycle', () => {
    // Login
    cy.visit('/login');
    cy.get('input[name="email"]').type('admin@example.com');
    cy.get('input[name="password"]').type('AdminPass123!');
    cy.get('button[type="submit"]').click();
    
    cy.url().should('include', '/dashboard');
    cy.contains('Welcome').should('be.visible');

    // Navigate to employees
    cy.get('a[routerLink="/employees"]').click();
    cy.url().should('include', '/employees');

    // Create employee
    cy.get('button:contains("Create Employee")').click();
    cy.url().should('include', '/employees/new');

    cy.get('input[name="firstName"]').type('Jane');
    cy.get('input[name="lastName"]').type('Smith');
    cy.get('input[name="email"]').type('jane@example.com');
    cy.get('select[name="department"]').select('IT');
    cy.get('input[name="password"]').type('NewPass123!');
    cy.get('input[name="confirmPassword"]').type('NewPass123!');
    cy.get('button[type="submit"]').click();

    // Should redirect to detail page and show success
    cy.url().should('match', /\/employees\/\d+/);
    cy.get('[data-testid="success-toast"]').should('contain', 'created successfully');
    cy.contains('Jane Smith').should('be.visible');

    // Edit employee
    cy.get('button:contains("Edit")').click();
    cy.url().should('include', '/edit');
    
    cy.get('input[name="firstName"]').clear().type('Janet');
    cy.get('button[type="submit"]').click();

    cy.get('[data-testid="success-toast"]').should('contain', 'updated successfully');
    cy.contains('Janet Smith').should('be.visible');

    // Delete employee
    cy.get('button:contains("Delete")').click();
    cy.get('[role="dialog"]').should('be.visible');
    cy.get('[role="dialog"] button:contains("Confirm")').click();

    cy.url().should('include', '/employees');
    cy.get('[data-testid="success-toast"]').should('contain', 'deleted successfully');
  });

  it('should handle rate limiting', () => {
    cy.login('admin@example.com', 'AdminPass123!');
    cy.visit('/employees/new');

    // Rapidly submit form
    for (let i = 0; i < 3; i++) {
      cy.get('input[name="email"]').clear().type(`test${i}@example.com`);
      cy.get('button[type="submit"]').click();
    }

    // Should trigger rate limit
    cy.get('[data-testid="warning-toast"]').should('contain', 'Too many requests');
    cy.get('button[type="submit"]').should('be.disabled');

    // Wait for retry countdown
    cy.get('button[type="submit"]').contains(/Retry in \d+s/);
  });

  it('should auto-refresh expired token', () => {
    cy.login('user@example.com', 'UserPass123!');
    cy.visit('/employees');

    // Simulate token expiry by clearing it
    cy.window().then((win) => {
      win.localStorage.setItem('accessTokenExpiresAt', '0');
    });

    // Navigate away and back
    cy.visit('/dashboard');
    cy.visit('/employees');

    // Should work (auto-refreshed)
    cy.get('table').should('be.visible');
  });
});
```

**Permission Testing:**

```
describe('Role-Based Access Control', () => {
  
  it('should prevent non-admin from creating employee', () => {
    cy.login('user@example.com', 'UserPass123!');
    cy.visit('/employees');

    // Create button should not exist
    cy.get('button:contains("Create Employee")').should('not.exist');

    // Try direct URL
    cy.visit('/employees/new');
    cy.url().should('include', '/employees');
    // Should redirect back
  });

  it('should prevent non-HR admin from deleting', () => {
    cy.login('admin-nonhr@example.com', 'AdminPass123!');
    cy.visit('/employees/1');

    // Delete button should not exist
    cy.get('button:contains("Delete")').should('not.exist');

    // Edit button should exist
    cy.get('button:contains("Edit")').should('exist');
  });

  it('should allow HR admin to delete', () => {
    cy.login('admin-hr@example.com', 'AdminPass123!');
    cy.visit('/employees/1');

    // Both buttons should exist
    cy.get('button:contains("Edit")').should('exist');
    cy.get('button:contains("Delete")').should('exist');
  });
});
```

---

## SUMMARY & HANDOFF CHECKLIST

### Key Angular-Specific Deliverables

**Architecture Foundation:**
- ✅ Angular 18+ with TypeScript strict mode
- ✅ NgRx for state management (Store, Actions, Reducers, Effects, Selectors)
- ✅ HttpClient with multi-layer interceptor chain
- ✅ Feature modules with lazy loading
- ✅ CoreModule for singletons, SharedModule for reusables
- ✅ Guards, Resolvers for route protection
- ✅ Reactive Forms for validation

**Authentication System:**
- ✅ JWT in Authorization header (from NgRx store)
- ✅ Refresh token in HttpOnly cookie
- ✅ Silent + reactive refresh strategies
- ✅ AuthGuard, AdminGuard, PolicyGuard for routes
- ✅ Auth interceptor for token injection
- ✅ Error interceptor with 401/403/429 handling

**State Management (NgRx):**
- ✅ Auth feature state (user, tokens, loading)
- ✅ Employees feature state (entities, filters, pagination)
- ✅ UI feature state (notifications, modals, rate limits)
- ✅ Selectors for memoized state derivation
- ✅ Effects for side effects (API calls, redirects)

**API Integration:**
- ✅ ApiResponse wrapper unwrapping
- ✅ Correlation ID tracking
- ✅ Rate limit header parsing
- ✅ Idempotency key generation
- ✅ Request/response interceptor strategy

**User Interface:**
- ✅ 9 page structure (Login, Register, Dashboard, Employees CRUD, Admins, Profile)
- ✅ Master layout (header, sidebar, main)
- ✅ Async pipe for reactive rendering
- ✅ Directives for role-based UI (*appHasRole)
- ✅ Loading states (ng-template with *ngIf)
- ✅ Error states (from store.select)
- ✅ Empty states
- ✅ Integrate SSE in both Front And Backend on CSV file upload
- ✅ CSV file upload option if admin is HR admin
- ✅ Proper working Dashboard
- ✅ it should have all the feature a enterprise level EMp management system should have.
- ✅ Admins should have feature and allow to do stuff according to their policiesrelated to their departments, add policies in backend like Enterprise grade should be using.

**Security:**
- ✅ XSS prevention (Angular sanitization, CSP)
- ✅ CSRF protection (SameSite=Strict, CORS)
- ✅ HTTPOnly cookies for refresh token
- ✅ DomSanitizer for trusted HTML
- ✅ Dependency updates automation

**Performance:**
- ✅ Lazy loading modules
- ✅ Preloading strategy
- ✅ OnPush change detection
- ✅ Async pipe (automatic unsubscribe)
- ✅ takeUntil pattern for subscriptions
- ✅ HTTP caching interceptor
- ✅ Virtual scrolling support

**Testing:**
- ✅ Unit tests (Reducers, Effects, Services, Directives)
- ✅ Integration tests (Components, Error interceptor)
- ✅ E2E tests (Cypress/Playwright for user journeys)
- ✅ MockStore for testing without backend
