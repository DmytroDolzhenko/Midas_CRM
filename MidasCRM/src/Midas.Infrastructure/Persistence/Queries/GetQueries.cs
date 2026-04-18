using Infrastructure.Persistence;
using Midas.Application.Common.Interfaces.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Midas.Core;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class GetQueries<T, TKey> : IGetQueries<T, TKey> where T : class, IEntity<TKey>
    {
        private readonly ApplicationDbContext _context;

        public GetQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<T>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }
    }
}
