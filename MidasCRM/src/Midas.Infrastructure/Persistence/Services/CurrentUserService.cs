using Microsoft.AspNetCore.Http;
using Midas.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Services
{
    public class CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext dbContext) : ICurrentUserService
    {
        public Guid? UserId
        {
            get
            {
                var user = httpContextAccessor.HttpContext?.User;
                var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub);

                return Guid.TryParse(userIdClaim, out var id) ? id : null;
            }
        }

        public async Task<Guid?> GetCompanyIdAsync(CancellationToken cancellationToken)
        {
            if (UserId is null)
            {
                return null;
            }

            var availableCompanyIds = await GetAvailableCompanyIdsAsync(cancellationToken);
            if (availableCompanyIds.Count == 0)
            {
                return null;
            }

            var requestedCompanyId = httpContextAccessor.HttpContext?.Request.Headers["X-Company-Id"].FirstOrDefault();
            if (Guid.TryParse(requestedCompanyId, out var parsedCompanyId)
                && availableCompanyIds.Contains(parsedCompanyId))
            {
                return parsedCompanyId;
            }

            return availableCompanyIds[0];
        }

        public async Task<IReadOnlyList<Guid>> GetAvailableCompanyIdsAsync(CancellationToken cancellationToken)
        {
            if (UserId is null)
            {
                return Array.Empty<Guid>();
            }

            return await dbContext.Set<Midas.Core.CompanyMembers.CompanyMember>()
                .Where(x => x.UserId == UserId.Value)
                .OrderBy(x => x.JoinedAtUtc)
                .Select(x => x.CompanyId)
                .ToListAsync(cancellationToken);
        }
    }
}
