using Access.Domain.Entities.Employees;
using Access.Domain.Entities.Employees.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Persistence.Configurations
{
    public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(employee => employee.Id);
            builder.Property(employee => employee.Id).ValueGeneratedNever();

            builder.Property(employee => employee.DocumentNumber)
                .HasConversion(document => document.Value, value => new DocumentNumber(value))
                .HasMaxLength(DocumentNumber.MaxLength).IsRequired();
            builder.Property(employee => employee.Email)
                .HasConversion(email => email.Value, value => new Email(value))
                .HasMaxLength(Email.MaxLength).IsRequired();
            builder.HasIndex(employee => employee.DocumentNumber).IsUnique();
            builder.HasIndex(employee => employee.Email).IsUnique();

            builder.Property(employee => employee.FirstName).HasMaxLength(Employee.NameMaxLength).IsRequired();
            builder.Property(employee => employee.LastName).HasMaxLength(Employee.NameMaxLength).IsRequired();
            builder.Property(employee => employee.Position).HasMaxLength(Employee.PositionMaxLength).IsRequired();
            builder.Property(employee => employee.Status).HasConversion<int>().IsRequired();
            builder.Property(employee => employee.CreatedAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");
            builder.Property(employee => employee.DeactivatedAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");
            builder.Ignore(employee => employee.IsActive);

            builder.HasOne(employee => employee.Area).WithMany(area => area.Employees)
                .HasForeignKey(employee => employee.AreaId).OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
