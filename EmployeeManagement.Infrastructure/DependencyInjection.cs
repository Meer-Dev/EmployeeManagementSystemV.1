using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Infrastructure.Jobs;
using EmployeeManagement.Infrastructure.Logging;
using EmployeeManagement.Infrastructure.Persistence;
using EmployeeManagement.Infrastructure.Repositories;
using EmployeeManagement.Infrastructure.Services.Cache;
using EmployeeManagement.Infrastructure.Services.Identity;
using EmployeeManagement.Infrastructure.Services.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace EmployeeManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.CommandTimeout(120);
            }));

        // Redis — singleton multiplexer (resilient, abortConnect=false)
        var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnection));
        services.AddScoped<ICacheService, RedisCacheService>();

        // Hangfire CSV job
        services.AddTransient<ProcessEmployeeCsvJob>();

        services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();

        services.AddScoped<IExceptionLogger, DatabaseExceptionLogger>();

        services.AddScoped<IJwtService, JwtService>();

        services
            .AddIdentityCore<EmployeeManagement.Domain.Entities.Employee>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddUserStore<EmployeeUserStore>();

        services.AddScoped<EmployeeUserStore>();
        services.AddScoped<IEmployeeIdentityService, EmployeeIdentityService>();

        // =============================
        // 🔐 JWT CONFIGURATION START
        // =============================

        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        var jwtSection = configuration.GetSection("Jwt");

        // Validate JWT configuration exists
        if (!jwtSection.Exists())
            throw new InvalidOperationException(
                "JWT configuration section 'Jwt' not found in appsettings.json");

        var jwtOptions = jwtSection.Get<JwtOptions>();

        // Validate JWT options
        if (jwtOptions == null)
            throw new InvalidOperationException(
                "Failed to bind JWT configuration. Ensure appsettings.json has valid Jwt section.");

        if (string.IsNullOrWhiteSpace(jwtOptions.Key))
            throw new InvalidOperationException(
                "Jwt:Key cannot be null or empty in appsettings.json");

        if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
            throw new InvalidOperationException(
                "Jwt:Issuer cannot be null or empty in appsettings.json");

        if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
            throw new InvalidOperationException(
                "Jwt:Audience cannot be null or empty in appsettings.json");

        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

            options.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Unauthorized - Invalid or missing token"
                    });
                },

                OnForbidden = async context =>
                {
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        message = "Forbidden - You are not authorized"
                    });
                }
            };
        });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"))
            .AddPolicy("CanDeleteEmployee", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") &&
                    context.User.HasClaim("Department", "HR")));

        // =============================
        // 🔐 JWT CONFIGURATION END
        // =============================


        return services;
    }
}