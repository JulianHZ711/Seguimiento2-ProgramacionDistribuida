using Access.Domain.Entities.Credentials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Persistence.Configurations
{
    public sealed class CredentialConfiguration : IEntityTypeConfiguration<Credential>
    {
        public void Configure(EntityTypeBuilder<Credential> builder)
        {
            builder.ToTable("Credentials");
            builder.HasKey(credential => credential.Id);
            builder.Property(credential => credential.Id).ValueGeneratedNever();
            builder.Property(credential => credential.Code).HasMaxLength(Credential.CodeMaxLength).IsRequired();
            builder.HasIndex(credential => credential.Code).IsUnique();
            builder.Property(credential => credential.Status).HasConversion<int>().IsRequired();
            builder.Property(credential => credential.CreatedAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");
            builder.Property(credential => credential.BlockedAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");
            builder.Property(credential => credential.RevokedAt).HasConversion<UtcDateTimeConverter>().HasColumnType("datetime2");
            builder.Ignore(credential => credential.IsActive);

            builder.HasOne(credential => credential.Employee).WithMany()
                .HasForeignKey(credential => credential.EmployeeId).OnDelete(DeleteBehavior.ClientNoAction);
        }
    }
}
