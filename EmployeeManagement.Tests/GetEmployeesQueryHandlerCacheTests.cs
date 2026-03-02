using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Employees.Queries.GetEmployees;
using FluentAssertions;
using Moq;
using Xunit;

namespace EmployeeManagement.Tests.Employees.Queries;

public class GetEmployeesQueryHandlerCacheTests
{
    private readonly Mock<IEmployeeReadRepository> _repoMock = new();
    private readonly Mock<ICacheService> _cacheMock = new();

    private GetEmployeesQueryHandler CreateHandler() =>
        new(_repoMock.Object, _cacheMock.Object);

    private static PagedResult<EmployeeDto> SampleResult() => new()
    {
        Items = [new EmployeeDto(1, string.Empty, "Ali", "Khan", "ali@test.com", "IT", true)],
        TotalCount = 1,
        PageNumber = 1,
        PageSize = 20
    };

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedResult_AndDoesNotCallRepository()
    {
        // Arrange
        var query = new GetEmployeesQuery();
        var cached = SampleResult();

        _cacheMock
            .Setup(c => c.GetAsync<PagedResult<EmployeeDto>>(It.IsAny<string>()))
            .ReturnsAsync(cached);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(cached);
        _repoMock.Verify(r => r.GetPagedAsync(
            It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<int?>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_CacheMiss_CallsRepository_AndSetsCache()
    {
        // Arrange
        var query = new GetEmployeesQuery();
        var dbResult = SampleResult();

        _cacheMock
            .Setup(c => c.GetAsync<PagedResult<EmployeeDto>>(It.IsAny<string>()))
            .ReturnsAsync((PagedResult<EmployeeDto>?)null);

        _repoMock
            .Setup(r => r.GetPagedAsync(
                It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(),
                It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<int?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbResult);

        var handler = CreateHandler();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(dbResult);

        _cacheMock.Verify(c => c.SetAsync(
            It.IsAny<string>(),
            It.IsAny<PagedResult<EmployeeDto>>(),
            TimeSpan.FromMinutes(5)), Times.Once);
    }
}