using EmployeeManagement.Application.Employees.Commands.CreateEmployee;
using EmployeeManagement.Application.Employees.Events;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Application.Common.Interfaces;
using MediatR;
using Moq;
using Xunit;
using FluentAssertions;

namespace EmployeeManagement.Tests.Employees.Commands;

public class CreateEmployeeCommandHandlerTests
{
    private readonly Mock<IEmployeeIdentityService> _identityMock = new();
    private readonly Mock<IPublisher> _publisherMock = new();
    private readonly CreateEmployeeCommandHandler _handler;

    public CreateEmployeeCommandHandlerTests()
    {
        _publisherMock
            .Setup(p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _handler = new CreateEmployeeCommandHandler(_identityMock.Object, _publisherMock.Object);
    }

    [Fact]
    public async Task Should_Create_Employee_And_Return_Id()
    {
        var command = new CreateEmployeeCommand("Ali", "Khan", "ali@test.com", "IT", "Password123", "User");

        _identityMock
            .Setup(x => x.CreateEmployeeAsync(It.IsAny<Employee>(), command.Password, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(5);
    }

    [Fact]
    public async Task Should_Publish_EmployeeCreatedEvent_After_Create()
    {
        var command = new CreateEmployeeCommand("Ali", "Khan", "ali@test.com", "IT", "Password123", "User");

        _identityMock
            .Setup(x => x.CreateEmployeeAsync(It.IsAny<Employee>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(42);

        await _handler.Handle(command, CancellationToken.None);

        _publisherMock.Verify(p => p.Publish(
            It.Is<EmployeeCreatedEvent>(e => e.EmployeeId == 42 && e.Email == "ali@test.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Identity_Fails()
    {
        var command = new CreateEmployeeCommand("Ali", "Khan", "ali@test.com", "IT", "Password123", "User");

        _identityMock
            .Setup(x => x.CreateEmployeeAsync(It.IsAny<Employee>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Failed"));

        await Assert.ThrowsAsync<Exception>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Not_Publish_Event_When_Identity_Fails()
    {
        var command = new CreateEmployeeCommand("Ali", "Khan", "ali@test.com", "IT", "Password123", "User");

        _identityMock
            .Setup(x => x.CreateEmployeeAsync(It.IsAny<Employee>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Failed"));

        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _publisherMock.Verify(p => p.Publish(
            It.IsAny<EmployeeCreatedEvent>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }
}