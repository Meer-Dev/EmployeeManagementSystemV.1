using EmployeeManagement.API.Common;
using EmployeeManagement.API.Middleware;
using EmployeeManagement.Application;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Repositories;
using Hangfire;
using Hangfire.MemoryStorage;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Logging
LoggingConfiguration.ConfigureLogging(builder);

// Controllers + Filters
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});


// Application + Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// UnitOfWork & Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();

//Hangfire addhangfire
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not configured");

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(connectionString, new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(15),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    });
});


// CORS
var corsOptions = builder.Configuration.GetSection("Cors");
var allowedOrigins = corsOptions.GetSection("AllowedOrigins").Get<string[]>();

//GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<EmployeeQuery>()
    .AddFiltering()
    .AddSorting();


// If credentials are allowed, you must provide explicit origins
var allowCredentials = corsOptions.GetValue<bool>("AllowCredentials", false);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        if (allowCredentials)
        {
            // Must explicitly list origins; wildcard is not allowed
            if (allowedOrigins == null || allowedOrigins.Length == 0)
                throw new InvalidOperationException("You must specify AllowedOrigins in appsettings.json if AllowCredentials is true.");

            policy.WithOrigins(allowedOrigins)
                  .AllowCredentials()
                  .WithHeaders("Content-Type", "Authorization", "Idempotency-Key")
                  .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS");
        }
        else
        {
            // Safe to allow wildcard
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

var app = builder.Build();

// Seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<EmployeeManagement.Domain.Entities.Employee>>();
    await SeedData.InitializeAsync(context, userManager);
}

// Middleware
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<IdempotencyKeyMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseCors("AllowSpecificOrigins");
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestLoggingMiddleware>();
app.MapGraphQL("/graphql"); // <-- THIS IS THE ROOT PATH


// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

// Map controllers
app.MapControllers();

app.Run();