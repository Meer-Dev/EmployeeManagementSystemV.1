using EmployeeManagement.Application.Common.Interfaces;
using EmployeeManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace EmployeeManagement.Tests
{
    public class TestAppDbContext : DbContext, IUnitOfWork
    {
        public TestAppDbContext(DbContextOptions<TestAppDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;

        IRepository<Employee> IUnitOfWork.Employees => throw new NotImplementedException();

        public IRepository<RefreshToken> RefreshTokens => throw new NotImplementedException();

        public Task<Employee?> GetEmployeeByIdAsync(int id, CancellationToken cancellationToken = default)
            => Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public void AddEmployee(Employee employee)
            => Employees.Add(employee);

        public void DeleteEmployee(Employee employee)
            => Employees.Remove(employee);

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    => base.SaveChangesAsync(cancellationToken);

        public Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}