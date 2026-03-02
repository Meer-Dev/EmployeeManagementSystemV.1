using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Application.Employees.Dtos;
using EmployeeManagement.Application.Employees.Queries.GetEmployees;
using EmployeeManagement.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace EmployeeManagement.Tests.Employees.Queries
{
    public class GetAdminsQueryHandlerTests
    {
        [Fact]
        public async Task Should_Return_Admins_List()
        {
            var options = new DbContextOptionsBuilder<TestAppDbContext>()
                .UseInMemoryDatabase(databaseName: "AdminsDb")
                .Options;

            await using var context = new TestAppDbContext(options);

            // Seed data
            context.AddEmployee(new Employee("Ali", "Khan", "ali@test.com", "IT", "", "Admin"));
            context.AddEmployee(new Employee("Ahmed", "Sheikh", "ahmed@test.com", "HR", "", "Admin"));
            context.AddEmployee(new Employee("Bilal", "Khan", "bilal@test.com", "IT", "", "User"));
            await context.SaveChangesAsync();

            var repoMock = new Mock<IRepository<Employee>>();

            var unitMock = new Mock<IUnitOfWork>();

            unitMock.Setup(x => x.Employees)
                .Returns(repoMock.Object);


           
        }
    }
}