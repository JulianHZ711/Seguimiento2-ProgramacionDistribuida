using Access.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Access.Persistence.UnitOfWorks
{
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;

        public EfCoreUnitOfWork(DataContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public Task RollbackAsync()
        {
            return Task.CompletedTask;
        }
    }
}
