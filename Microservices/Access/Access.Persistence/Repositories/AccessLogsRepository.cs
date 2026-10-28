using Microsoft.EntityFrameworkCore;
using Access.Application.Contracts.Repositories;
using Access.Application.Utilities.Pagination;
using Access.Domain.Entities.AccessLogs;
using Access.Domain.Entities.AccessLogs.ValueObjects;
using Access.Persistence.Extensions;

namespace Access.Persistence.Repositories
{
    public class AccessLogsRepository : Repository<AccessLog>, IAccessLogsRepository
    {
        private readonly DataContext _context;

        public AccessLogsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AccessLog>> GetPagedListAsync(PaginationRequest request,
                                                                           Guid? employeeId,
                                                                           DateTime? from,
                                                                           DateTime? to,
                                                                           Guid? accessPointId,
                                                                           AccessResult? result,
                                                                           CancellationToken cancellationToken = default)
        {
            IQueryable<AccessLog> query = _context.Set<AccessLog>()
                                                  .Include(l => l.Employee)
                                                  .Include(l => l.AccessPoint)
                                                  .AsQueryable();

            if (employeeId.HasValue)
            {
                query = query.Where(l => l.EmployeeId == employeeId);
            }

            if (from.HasValue)
            {
                query = query.Where(l => l.OccurredAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(l => l.OccurredAt <= to.Value);
            }

            if (accessPointId.HasValue)
            {
                query = query.Where(l => l.AccessPointId == accessPointId);
            }

            if (result.HasValue)
            {
                query = query.Where(l => l.Result == result);
            }

            query = query.OrderByDescending(l => l.OccurredAt)
                         .ThenByDescending(l => l.Id);

            return await query.ToPagedListAsync(request, cancellationToken);
        }

        public async Task<AccessLog?> GetLastAuthorizedByEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<AccessLog>().Where(l => l.EmployeeId == employeeId && l.Result == AccessResult.Authorized)
                                                  .OrderByDescending(l => l.OccurredAt)
                                                  .ThenByDescending(l => l.Id)
                                                  .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
