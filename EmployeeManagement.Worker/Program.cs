using EmployeeManagement.Application;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Infrastructure;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Repositories;
using Hangfire;
using Hangfire.Common;   // Needed for Job
using Hangfire.SqlServer;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWindowsService();

// ✅ Minimal routing for health checks (if needed)
builder.Services.AddRouting();
builder.Services.AddAuthorization();

// App + Infra
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();

// Hangfire

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection not configured");

builder.Services.AddHangfire(config =>
{
    config
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromSeconds(15),
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        });
    config.UseFilter(new AutomaticRetryAttribute { Attempts = 3 });
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = Environment.ProcessorCount * 2;
    options.Queues = new[] { "default", "critical" };
    options.ShutdownTimeout = TimeSpan.FromMinutes(30);
});

var host = builder.Build();

// ✅ Configure logging based on environment
var logger = host.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Worker starting in {Environment} environment",
    builder.Environment.EnvironmentName);

using (var scope = host.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate(
        recurringJobId: "DailyEmployeeReport",
        job: Job.FromExpression<IEmployeeReportService>(x => x.GenerateDailyReport()),
        cronExpression: "00 00 * * *", // Every day at midnight
        options: new RecurringJobOptions()
    );
}

host.Run();