using EmployeeManagement.Application.Employees.Commands.PatchEmployee;
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
    public class PatchEmployeeCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _contextMock;
        private readonly PatchEmployeeCommandHandler _handler;

        public PatchEmployeeCommandHandlerTests()
        {
            _contextMock = new Mock<IUnitOfWork>();
            _handler = new PatchEmployeeCommandHandler(_contextMock.Object);
        }

        [Fact]
        public async Task Should_Update_FirstName_Only()
        {
            var employee = new Employee("Ali", "Khan", "ali@test.com", "IT", "", "User");

            _contextMock.Setup(x => x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(employee);

            var command = new PatchEmployeeCommand(
    1,        // Id
    "Ahmed",  // FirstName
    null,     // LastName
    null,     // Email
    null,     // Department
    null      // IsActive
);

            await _handler.Handle(command, CancellationToken.None);

            employee.FirstName.Should().Be("Ahmed");
            employee.LastName.Should().Be("Khan");
        }

        [Fact]
        public async Task Should_Throw_When_Employee_Not_Found()
        {
            _contextMock.Setup(x => x.GetEmployeeByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Employee?)null);

            var command = new PatchEmployeeCommand(
    1,        // Id
    "Ahmed",  // FirstName
    null,     // LastName
    null,     // Email
    null,     // Department
    null      // IsActive
);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }
}