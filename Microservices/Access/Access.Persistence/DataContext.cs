using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.Areas;
using Access.Domain.Entities.Credentials;
using Access.Domain.Entities.Employees;
using Microsoft.EntityFrameworkCore;

namespace Access.Persistence
{
    public sealed class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Credential> Credentials => Set<Credential>();
        public DbSet<AccessPoint> AccessPoints => Set<AccessPoint>();
        public DbSet<AccessLog> AccessLogs => Set<AccessLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly);
        }
    }
}
