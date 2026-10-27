using Access.Domain.Entities.Credentials;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Contracts.Repositories
{
    public interface ICredentialsRepository : IRepository<Credential>
    {
        Task<Credential?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);

        Task<Credential?> GetByCodeWithEmployeeAsync(string code, CancellationToken cancellationToken = default);

        Task<IEnumerable<Credential>> GetByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default);

        Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
    }
}
