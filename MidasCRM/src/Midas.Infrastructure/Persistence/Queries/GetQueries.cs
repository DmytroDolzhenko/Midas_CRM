using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;

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
            var query = _context.Set<T>().AsQueryable();

            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(entity => !EF.Property<bool>(entity, "IsDeleted"));
            }

            if (typeof(T) == typeof(Company))
            {
                var currentUserId = _currentUserService.UserId;
                if (currentUserId is null)
                {
                    return query.Where(_ => false);
                }

                return query.Where(entity =>
                    _context.Set<CompanyMember>().Any(member =>
                        member.CompanyId == EF.Property<Guid>(entity, nameof(Company.Id))
                        && member.UserId == currentUserId.Value));
            }

            if (!typeof(ICompanyOwnedEntity).IsAssignableFrom(typeof(T)))
            {
                return query;
            }

            var companyId = await _currentUserService.GetCompanyIdAsync(cancellationToken);
            if (companyId is null)
            {
                return query.Where(_ => false);
            }

            return query.Where(entity => EF.Property<Guid>(entity, nameof(ICompanyOwnedEntity.CompanyId)) == companyId.Value);
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

