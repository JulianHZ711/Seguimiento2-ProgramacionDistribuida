using Microsoft.EntityFrameworkCore;
using Access.Application.Contracts.Repositories;
using Access.Application.Utilities.Pagination;
using Access.Domain.Entities.Employees;
using Access.Domain.Entities.Employees.ValueObjects;
using Access.Persistence.Extensions;

namespace Access.Persistence.Repositories
{
    public class EmployeesRepository : Repository<Employee>, IEmployeesRepository
    {
        private readonly DataContext _context;

        public EmployeesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Employee?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Employee>().Include(e => e.Area)
                                                 .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<PaginationResponse<Employee>> GetPagedListAsync(PaginationRequest request,
                                                                          Guid? areaId,
                                                                          EmploymentStatus? status,
                                                                          CancellationToken cancellationToken = default)
        {
            IQueryable<Employee> query = _context.Set<Employee>()
                                                 .Include(e => e.Area)
                                                 .AsQueryable();

            if (areaId.HasValue)
            {
                query = query.Where(e => e.AreaId == areaId);
            }

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status);
            }

            query = query.OrderByDescending(e => e.CreatedAt)
                         .ThenBy(e => e.Id);

            return await query.ToPagedListAsync(request, cancellationToken);
        }

        public async Task<bool> ExistsByDocumentNumberAsync(DocumentNumber documentNumber, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Employee>().AnyAsync(e => e.DocumentNumber == documentNumber, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(Email email, Guid? excludeEmployeeId = null, CancellationToken cancellationToken = default)
        {
            IQueryable<Employee> query = _context.Set<Employee>().Where(e => e.Email == email);

            if (excludeEmployeeId.HasValue)
            {
                query = query.Where(e => e.Id != excludeEmployeeId);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}
