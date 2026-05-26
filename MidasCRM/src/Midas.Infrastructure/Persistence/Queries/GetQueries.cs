using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using Midas.Infrastructure.Persistence.Queries.Extensions;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class GetQueries<T, TKey> : IGetQueries<T, TKey> where T : class, IEntity<TKey>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GetQueries(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        private async Task<IQueryable<T>> BuildOwnedQueryAsync(CancellationToken cancellationToken)
        {
            var companyId = await _currentUserService.GetCompanyIdAsync(cancellationToken);
            var userId = _currentUserService.UserId;

            return _context.Set<T>()
                .AsQueryable()
                .ApplyCompanyFilter(_context, userId, companyId);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken,
            Func<IQueryable<T>, IQueryable<T>>? queryShaper = null)
        {
            var query = (await BuildOwnedQueryAsync(cancellationToken)).AsNoTracking();
            if (queryShaper is not null)
            {
                query = queryShaper(query);
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(
            TKey id,
            CancellationToken cancellationToken,
            Func<IQueryable<T>, IQueryable<T>>? queryShaper = null)
        {
            var query = await BuildOwnedQueryAsync(cancellationToken);
            if (queryShaper is not null)
            {
                query = queryShaper(query);
            }

            return await query.FirstOrDefaultAsync(entity => entity.Id!.Equals(id), cancellationToken);
        }
    }
}

