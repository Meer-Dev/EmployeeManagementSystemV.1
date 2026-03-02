using EmployeeManagement.Application.Employees.Queries.GetEmployeeById;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Common.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace EmployeeManagement.Tests.Employees.Queries
{
    public class GetEmployeeByIdQueryHandlerTests
    {
        private readonly Mock<IEmployeeReadRepository> _repoMock;
        private readonly GetEmployeeByIdQueryHandler _handler;

        public GetEmployeeByIdQueryHandlerTests()
        {
            _repoMock = new Mock<IEmployeeReadRepository>();
            _handler = new GetEmployeeByIdQueryHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Should_Return_EmployeeDto()
        {
            var dto = new EmployeeDto(1, "", "Ali", "Khan", "ali@test.com", "IT", true);

            _repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(dto);

            var result = await _handler.Handle(new GetEmployeeByIdQuery(1), CancellationToken.None);

            result.Should().Be(dto);
        }

        [Fact]
        public async Task Should_Throw_When_Employee_Not_Found()
        {
            _repoMock.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync((EmployeeDto?)null);

            await Assert.ThrowsAsync<System.Collections.Generic.KeyNotFoundException>(() =>
                _handler.Handle(new GetEmployeeByIdQuery(1), CancellationToken.None));
        }
    }
}