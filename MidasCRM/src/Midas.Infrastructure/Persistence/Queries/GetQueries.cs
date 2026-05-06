using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core;

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

        private IQueryable<T> BuildOwnedQuery()
        {
            var query = _context.Set<T>().AsQueryable();
            if (!typeof(IOwnedEntity).IsAssignableFrom(typeof(T)))
            {
                return query;
            }

            var currentUserId = _currentUserService.UserId;
            if (currentUserId is null)
            {
                return query.Where(_ => false);
            }

            return query.Where(entity => EF.Property<Guid>(entity, nameof(IOwnedEntity.OwnerId)) == currentUserId.Value);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await BuildOwnedQuery()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await BuildOwnedQuery()
                .FirstOrDefaultAsync(entity => entity.Id!.Equals(id), cancellationToken);
        }
    }
}
