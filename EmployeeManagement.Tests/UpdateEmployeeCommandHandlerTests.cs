using EmployeeManagement.Application.Employees.Commands.UpdateEmployee;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace EmployeeManagement.Tests.Employees.Commands
{
    public class UpdateEmployeeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _contextMock;
        private readonly UpdateEmployeeCommandHandler _handler;

        public UpdateEmployeeCommandHandlerTests()
        {
            _contextMock = new Mock<IUnitOfWork>();
            _handler = new UpdateEmployeeCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Should_Update_All_Fields()
        {
            var employee = new Employee("Ali", "Khan", "ali@test.com", "IT", "", "User");

            _contextMock.Setup(x =>
        x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
    .ReturnsAsync(employee);

            var command = new UpdateEmployeeCommand(
    1,
    "Ahmed",
    "Sheikh",
    "ahmed@test.com",
    "HR",
    true
);

            await _handler.Handle(command, CancellationToken.None);

            employee.FirstName.Should().Be("Ahmed");
            employee.LastName.Should().Be("Sheikh");
            employee.Email.Should().Be("ahmed@test.com");
            employee.Department.Should().Be("HR");
        }

        [Fact]
        public async Task Should_Throw_When_Employee_Not_Found()
        {
            _contextMock.Setup(x =>
                x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            var command = new UpdateEmployeeCommand(
                1,
                "Ahmed",
                "Sheikh",
                "ahmed@test.com",
                "HR",
                true
            );

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            _contextMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}