using EmployeeManagement.Application.Common;
using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Employees.Queries.GetEmployees;
using FluentAssertions;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Employees.Queries
{
    public class GetEmployeesQueryHandlerTests
    {
        private readonly Mock<IEmployeeReadRepository> _repoMock;
        private readonly Mock<ICacheService> _cacheMock;
        private readonly GetEmployeesQueryHandler _handler;

        public GetEmployeesQueryHandlerTests()
        {
            _repoMock = new Mock<IEmployeeReadRepository>();
            _cacheMock = new Mock<ICacheService>();

            // Cache should return null to simulate cache miss
            _cacheMock
                .Setup(x => x.GetAsync<PagedResult<EmployeeDto>>(It.IsAny<string>()))
                .ReturnsAsync((PagedResult<EmployeeDto>)null);

            _handler = new GetEmployeesQueryHandler(
                _repoMock.Object,
                _cacheMock.Object);
        }

        [Fact]
        public async Task Should_Return_PagedResult()
        {
            var paged = new PagedResult<EmployeeDto>
            {
                Items = new List<EmployeeDto>(),
                TotalCount = 0,
                LastSeenId = null
            };

            _repoMock.Setup(x => x.GetPagedAsync(
                It.IsAny<int?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int?>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(paged);

            var result = await _handler.Handle(
                new GetEmployeesQuery(),
                CancellationToken.None);

            result.Should().Be(paged);
        }
    }
}