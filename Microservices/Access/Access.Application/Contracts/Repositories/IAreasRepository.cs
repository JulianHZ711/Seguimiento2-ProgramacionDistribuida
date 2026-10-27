using Access.Domain.Entities.Areas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Contracts.Repositories
{
    public interface IAreasRepository : IRepository<Area>
    {
        Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
