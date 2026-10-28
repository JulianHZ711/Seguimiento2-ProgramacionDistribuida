using Microsoft.EntityFrameworkCore;
using Access.Domain.Entities.Areas;

namespace Access.Persistence.Seeds.Areas
{
    public class AreaSeeder : IDataSeeder
    {
        private readonly DataContext _context;

        public AreaSeeder(DataContext context)
        {
            _context = context;
        }

        public int Order => 1;

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            if (await _context.Areas.AnyAsync(cancellationToken))
            {
                return;
            }

            List<Area> areas =
            [
                new Area("Administración", "Gerencia, contabilidad y gestión administrativa"),
                new Area("Talento Humano", "Gestión del personal y vinculación laboral"),
                new Area("Logística", "Planeación y despacho de operaciones logísticas"),
                new Area("Almacenamiento", "Recepción, custodia y control de inventario en bodega"),
                new Area("Seguridad", "Control de accesos y vigilancia de las instalaciones"),
                new Area("Tecnología", "Soporte de sistemas e infraestructura tecnológica"),
            ];

            await _context.Areas.AddRangeAsync(areas, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
