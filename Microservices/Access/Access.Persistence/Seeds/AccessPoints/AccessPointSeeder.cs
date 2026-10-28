using Microsoft.EntityFrameworkCore;
using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.AccessPoints.ValueObjects;

namespace Access.Persistence.Seeds.AccessPoints
{
    public class AccessPointSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public AccessPointSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 2;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.AccessPoints.AnyAsync(cancellationToken))
            {
                return;
            }

            List<AccessPoint> accessPoints =
            [
                new AccessPoint("Recepción principal", AccessPointType.Reception),
                new AccessPoint("Parqueadero", AccessPointType.Parking),
                new AccessPoint("Bodega principal", AccessPointType.Warehouse),
                new AccessPoint("Cuarto de servidores", AccessPointType.RestrictedArea),
                new AccessPoint("Bodega de alto valor", AccessPointType.RestrictedArea, isEnabled: false),
            ];

            await _context.AccessPoints.AddRangeAsync(accessPoints, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
