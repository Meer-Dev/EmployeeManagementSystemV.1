using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Events;
using EmployeeManagement.Infrastructure.Jobs;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Jobs;

public class ProcessEmployeeCsvJobTests : IDisposable
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();
    private readonly Mock<ILogger<ProcessEmployeeCsvJob>> _loggerMock = new();
    private readonly Mock<IRepository<EmployeeManagement.Domain.Entities.Employee>> _empRepo = new();
    private string? _tempFile;

    public ProcessEmployeeCsvJobTests()
    {
        _uowMock.Setup(u => u.Employees).Returns(_empRepo.Object);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _empRepo
            .Setup(r => r.AddAsync(It.IsAny<EmployeeManagement.Domain.Entities.Employee>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EmployeeManagement.Domain.Entities.Employee e, CancellationToken _) => e);
        _publisherMock
            .Setup(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private string WriteTemp(string content)
    {
        _tempFile = System.IO.Path.GetTempFileName() + ".csv";
        File.WriteAllText(_tempFile, content);
        return _tempFile;
    }

    [Fact]
    public async Task ExecuteAsync_ValidCsv_SavesEmployeesAndPublishesEvents()
    {
        var csv = "FirstName,LastName,Email,Department,IsActive\n" +
                  "Ali,Khan,ali@test.com,IT,true\n" +
                  "Sara,Ahmed,sara@test.com,HR,true";

        var path = WriteTemp(csv);

        var job = new ProcessEmployeeCsvJob(_uowMock.Object, _publisherMock.Object, _loggerMock.Object);
        await job.ExecuteAsync(path);

        _empRepo.Verify(r => r.AddAsync(
            It.IsAny<EmployeeManagement.Domain.Entities.Employee>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));

        _publisherMock.Verify(p => p.Publish(
            It.IsAny<EmployeeCreatedEvent>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ExecuteAsync_RowMissingEmail_SkipsRowAndContinues()
    {
        var csv = "FirstName,LastName,Email,Department,IsActive\n" +
                  "Bad,Row,,IT,true\n" +                          // no email — should skip
                  "Sara,Ahmed,sara@test.com,HR,true";

        var path = WriteTemp(csv);

        var job = new ProcessEmployeeCsvJob(_uowMock.Object, _publisherMock.Object, _loggerMock.Object);
        await job.ExecuteAsync(path);

        // Only valid row saved
        _empRepo.Verify(r => r.AddAsync(
            It.IsAny<EmployeeManagement.Domain.Entities.Employee>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_FileNotFound_LogsErrorAndReturns()
    {
        var job = new ProcessEmployeeCsvJob(_uowMock.Object, _publisherMock.Object, _loggerMock.Object);
        // Should not throw
        await job.Invoking(j => j.ExecuteAsync("C:/nonexistent/file.csv"))
            .Should().NotThrowAsync();

        _empRepo.Verify(r => r.AddAsync(
            It.IsAny<EmployeeManagement.Domain.Entities.Employee>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_DeletesTempFile_AfterProcessing()
    {
        var csv = "FirstName,LastName,Email,Department,IsActive\n" +
                  "Ali,Khan,ali@test.com,IT,true";
        var path = WriteTemp(csv);
        _tempFile = null; // job will delete it; no need for Dispose cleanup

        var job = new ProcessEmployeeCsvJob(_uowMock.Object, _publisherMock.Object, _loggerMock.Object);
        await job.ExecuteAsync(path);

        File.Exists(path).Should().BeFalse("temp file must be cleaned up by the job");
    }

    public void Dispose()
    {
        if (_tempFile != null && File.Exists(_tempFile))
            File.Delete(_tempFile);
    }
}