using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Persistence.Configurations
{
    public sealed class AccessLogConfiguration : IEntityTypeConfiguration<AccessLog>
    {
        public void Configure(EntityTypeBuilder<AccessLog> builder)
        {
            builder.ToTable("AccessLogs");
            builder.HasKey(log => log.Id);
            builder.Property(log => log.Id).ValueGeneratedNever();
            builder.Property(log => log.CredentialCode).HasMaxLength(Credential.CodeMaxLength).IsRequired();
            builder.Property(log => log.MovementType).HasConversion<int>().IsRequired();
            builder.Property(log => log.Result).HasConversion<int>().IsRequired();
            builder.Property(log => log.Reason).HasMaxLength(AccessLog.ReasonMaxLength);
            builder.Property(log => log.OccurredAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");

            // Conserva el historial y sus referencias: sin borrado en cascada ni desvinculación automática.
            builder.HasOne(log => log.Employee).WithMany()
                .HasForeignKey(log => log.EmployeeId).IsRequired(false).OnDelete(DeleteBehavior.ClientNoAction);
            builder.HasOne(log => log.Credential).WithMany()
                .HasForeignKey(log => log.CredentialId).IsRequired(false).OnDelete(DeleteBehavior.ClientNoAction);
            builder.HasOne(log => log.AccessPoint).WithMany()
                .HasForeignKey(log => log.AccessPointId).IsRequired().OnDelete(DeleteBehavior.ClientNoAction);

            builder.HasIndex(log => new { log.EmployeeId, log.OccurredAt });
            builder.HasIndex(log => log.OccurredAt);
        }
    }
}
