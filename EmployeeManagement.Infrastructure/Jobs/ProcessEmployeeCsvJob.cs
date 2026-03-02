using CsvHelper;
using CsvHelper.Configuration;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Events;
using EmployeeManagement.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace EmployeeManagement.Infrastructure.Jobs;

public class ProcessEmployeeCsvJob(
    IUnitOfWork unitOfWork,
    IPublisher publisher,
    ILogger<ProcessEmployeeCsvJob> logger)
{
    public async Task ExecuteAsync(string filePath)
    {
        logger.LogInformation("CSV job started. File: {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            logger.LogError("CSV file not found: {FilePath}", filePath);
            return;
        }

        var successCount = 0;
        var errorCount = 0;

        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);

            // ✅ Load ALL existing emails once
            var existingEmployees = await unitOfWork.Employees.GetAllAsync();
            var existingEmails = existingEmployees
                .Select(e => ((Employee)e).Email)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var employeesToInsert = new List<Employee>();

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
                try
                {
                    var firstName = csv.GetField<string>("FirstName") ?? string.Empty;
                    var lastName = csv.GetField<string>("LastName") ?? string.Empty;
                    var email = csv.GetField<string>("Email") ?? string.Empty;
                    var department = csv.GetField<string>("Department") ?? string.Empty;
                    var isActive = csv.GetField<bool?>("IsActive") ?? true;

                    if (string.IsNullOrWhiteSpace(email))
                    {
                        errorCount++;
                        continue;
                    }

                    // ✅ In-memory duplicate check (VERY FAST)
                    if (existingEmails.Contains(email))
                    {
                        logger.LogWarning("Duplicate email {Email}. Skipping.", email);
                        errorCount++;
                        continue;
                    }

                    var employee = new Employee(firstName, lastName, email, department, isActive);
                    employeesToInsert.Add(employee);

                    // Add to HashSet to avoid duplicates within same CSV
                    existingEmails.Add(email);
                }
                catch (Exception rowEx)
                {
                    logger.LogWarning(rowEx,
                        "Failed to process row {Row}. Skipping.",
                        csv.Context?.Parser?.Row);

                    errorCount++;
                }
            }

            // ✅ Bulk insert once
            if (employeesToInsert.Count > 0)
            {
                foreach (var emp in employeesToInsert)
                    await unitOfWork.Employees.AddAsync(emp);

                await unitOfWork.SaveChangesAsync();

                foreach (var employee in employeesToInsert)
                {
                    await publisher.Publish(
                        new EmployeeCreatedEvent(employee.Id, employee.Email));
                }

                successCount = employeesToInsert.Count;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "CSV job failed unexpectedly.");
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            logger.LogInformation(
                "CSV job finished. Processed={Success}, Errors={Errors}, File={FilePath}",
                successCount, errorCount, filePath);
        }
    }
}