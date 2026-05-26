using Microsoft.EntityFrameworkCore;
using Midas.Core;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyCompanyFilter<T>(
            this IQueryable<T> query,
            DbContext context,
            Guid? currentUserId,
            Guid? currentCompanyId) where T : class
        {
            if (typeof(T).GetProperty("IsDeleted") != null)
            {
                query = query.Where(entity => !EF.Property<bool>(entity, "IsDeleted"));
            }

            if (typeof(T) == typeof(Company))
            {
                if (currentUserId is null)
                {
                    return query.Where(_ => false);
                }

                return query.Where(entity =>
                    context.Set<CompanyMember>().Any(member =>
                        member.CompanyId == EF.Property<Guid>(entity, nameof(Company.Id))
                        && member.UserId == currentUserId.Value));
            }

            if (!typeof(ICompanyOwnedEntity).IsAssignableFrom(typeof(T)))
            {
                return query;
            }

            if (currentCompanyId is null)
            {
                return query.Where(_ => false);
            }

            return query.Where(entity => EF.Property<Guid>(entity, nameof(ICompanyOwnedEntity.CompanyId)) == currentCompanyId.Value);
        }
    }
}
