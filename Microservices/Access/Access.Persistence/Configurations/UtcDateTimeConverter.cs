using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Access.Persistence.Configurations
{
    // SQL Server no conserva DateTime.Kind; las fechas del dominio se almacenan en UTC.
    internal sealed class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
    {
        public UtcDateTimeConverter()
            : base(value => value, value => DateTime.SpecifyKind(value, DateTimeKind.Utc))
        {
        }
    }
}
