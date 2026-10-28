using Microsoft.EntityFrameworkCore;
using Access.Application.Contracts.Repositories;
using Access.Application.Utilities.Pagination;
using Access.Domain.Entities.AccessPoints;
using Access.Domain.Entities.AccessPoints.ValueObjects;
using Access.Persistence.Extensions;

namespace Access.Persistence.Repositories
{
    public class AccessPointsRepository : Repository<AccessPoint>, IAccessPointsRepository
    {
        private readonly DataContext _context;

        public AccessPointsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AccessPoint>> GetPagedListAsync(PaginationRequest request,
                                                                             bool? isEnabled,
                                                                             AccessPointType? type,
                                                                             CancellationToken cancellationToken = default)
        {
            IQueryable<AccessPoint> query = _context.Set<AccessPoint>().AsQueryable();

            if (isEnabled.HasValue)
            {
                query = query.Where(p => p.IsEnabled == isEnabled);
            }

            if (type.HasValue)
            {
                query = query.Where(p => p.Type == type);
            }

            query = query.OrderBy(p => p.Name)
                         .ThenBy(p => p.Id);

            return await query.ToPagedListAsync(request, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            string trimmedName = name.Trim();

            return await _context.Set<AccessPoint>().AnyAsync(p => p.Name == trimmedName, cancellationToken);
        }
    }
}
