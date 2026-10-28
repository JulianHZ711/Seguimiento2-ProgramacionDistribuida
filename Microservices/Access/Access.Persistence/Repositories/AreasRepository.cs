using Microsoft.EntityFrameworkCore;
using Access.Application.Contracts.Repositories;
using Access.Domain.Entities.Areas;

namespace Access.Persistence.Repositories
{
    public class AreasRepository : Repository<Area>, IAreasRepository
    {
        private readonly DataContext _context;

        public AreasRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Area>().AnyAsync(a => a.Id == id, cancellationToken);
        }
    }
}
