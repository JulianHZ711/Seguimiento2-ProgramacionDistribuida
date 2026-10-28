using Microsoft.EntityFrameworkCore;
using Access.Application.Contracts.Repositories;
using Access.Domain.Entities.Credentials;

namespace Access.Persistence.Repositories
{
    public class CredentialsRepository : Repository<Credential>, ICredentialsRepository
    {
        private readonly DataContext _context;

        public CredentialsRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Credential?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            string normalizedCode = NormalizeCode(code);

            return await _context.Set<Credential>().FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);
        }

        public async Task<Credential?> GetByCodeWithEmployeeAsync(string code, CancellationToken cancellationToken = default)
        {
            string normalizedCode = NormalizeCode(code);

            return await _context.Set<Credential>().Include(c => c.Employee)
                                                   .FirstOrDefaultAsync(c => c.Code == normalizedCode, cancellationToken);
        }

        public async Task<IEnumerable<Credential>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
        {
            return await _context.Set<Credential>().Where(c => c.EmployeeId == employeeId)
                                                   .OrderByDescending(c => c.CreatedAt)
                                                   .ThenBy(c => c.Id)
                                                   .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            string normalizedCode = NormalizeCode(code);

            return await _context.Set<Credential>().AnyAsync(c => c.Code == normalizedCode, cancellationToken);
        }

        // El dominio guarda el código sin espacios y en mayúsculas; se busca de la misma forma.
        private static string NormalizeCode(string code)
        {
            return string.IsNullOrWhiteSpace(code) ? string.Empty : code.Trim().ToUpperInvariant();
        }
    }
}
