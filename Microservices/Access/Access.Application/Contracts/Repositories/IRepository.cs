using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Application.Contracts.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);
    }
}
