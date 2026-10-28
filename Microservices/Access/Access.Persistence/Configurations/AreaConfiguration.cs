using Access.Domain.Entities.Areas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Access.Persistence.Configurations
{
    public sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("Areas");
            builder.HasKey(area => area.Id);
            builder.Property(area => area.Id).ValueGeneratedNever();
            builder.Property(area => area.Name).HasMaxLength(Area.NameMaxLength).IsRequired();
            builder.Property(area => area.Description).HasMaxLength(Area.DescriptionMaxLength);
        }
    }
}
