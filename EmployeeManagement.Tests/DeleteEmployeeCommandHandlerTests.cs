using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Commands.DeleteEmployee;
using EmployeeManagement.Domain.Entities;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace EmployeeManagement.Tests.Employees.Commands
{
    public class DeleteEmployeeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _contextMock;
        private readonly DeleteEmployeeCommandHandler _handler;

        public DeleteEmployeeCommandHandlerTests()
        {
            _contextMock = new Mock<IUnitOfWork>();
            _handler = new DeleteEmployeeCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Should_Throw_When_Employee_Not_Found()
        {
            _contextMock.Setup(x =>
                x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(new DeleteEmployeeCommand(1), CancellationToken.None));

            _contextMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Throw_When_Already_Inactive()
        {
            var employee = new Employee("Ali", "Khan", "ali@test.com", "IT", "", "User");
            employee.Deactivate();

            _contextMock.Setup(x =>
                x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(new DeleteEmployeeCommand(1), CancellationToken.None));

            _contextMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Should_Deactivate_Employee()
        {
            var employee = new Employee("Ali", "Khan", "ali@test.com", "IT", "", "User");

            _contextMock.Setup(x =>
                x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            await _handler.Handle(new DeleteEmployeeCommand(1), CancellationToken.None);

            employee.IsActive.Should().BeFalse();

            _contextMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}