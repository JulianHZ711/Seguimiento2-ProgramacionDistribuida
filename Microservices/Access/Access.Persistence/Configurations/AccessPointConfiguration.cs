using Access.Domain.Entities.AccessPoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Persistence.Configurations
{
    public sealed class AccessPointConfiguration : IEntityTypeConfiguration<AccessPoint>
    {
        public void Configure(EntityTypeBuilder<AccessPoint> builder)
        {
            builder.ToTable("AccessPoints");
            builder.HasKey(point => point.Id);
            builder.Property(point => point.Id).ValueGeneratedNever();
            builder.Property(point => point.Name).HasMaxLength(AccessPoint.NameMaxLength).IsRequired();
            builder.Property(point => point.Type).HasConversion<int>().IsRequired();
            builder.Property(point => point.IsEnabled).IsRequired();
        }
    }
}
