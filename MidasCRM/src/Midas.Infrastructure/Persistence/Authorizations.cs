using Application.Common.Securities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Midas.Core.Users;

namespace Infrastructure.Persistence
{
    public class NotDeletedHandler(UserManager<User> userManager) : AuthorizationHandler<NotDeletedRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            NotDeletedRequirement requirement)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub);

            if (userIdClaim == null) return;

            var user = await userManager.FindByIdAsync(userIdClaim.Value);

            if (user != null && !user.IsDeleted)
            {
                context.Succeed(requirement);
            }
        }
    }
}