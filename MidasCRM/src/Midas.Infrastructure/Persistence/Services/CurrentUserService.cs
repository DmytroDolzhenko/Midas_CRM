using Microsoft.AspNetCore.Http;
using Midas.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
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
    }
}
