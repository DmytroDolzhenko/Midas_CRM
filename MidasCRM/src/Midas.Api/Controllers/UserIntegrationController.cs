using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midas.Api.DTOs;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Entities.ConnectionServices.Commands;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserIntegrationController(
        ISender sender,
        ICurrentUserService currentUser,
        IEnumerable<IIntegrationProvider> providers,
        IIntegrationStateService stateService,
        IConfiguration configuration) : ControllerBase
    {
        [Authorize]
        [HttpGet("{provider}/start")]
        public ActionResult StartConnect([FromRoute] string provider)
        {
            if (currentUser.UserId is null)
            {
                return Unauthorized();
            }

            var integrationProvider = providers.FirstOrDefault(x =>
                string.Equals(x.ProviderName, provider, StringComparison.OrdinalIgnoreCase));

            if (integrationProvider is null)
            {
                return NotFound($"Provider '{provider}' is not configured.");
            }

            var state = stateService.CreateState(currentUser.UserId.Value, provider);
            var authorizeUrl = integrationProvider.BuildAuthorizeUrl(state);

            return Redirect(authorizeUrl);
        }

        [AllowAnonymous]
        [HttpGet("{provider}/callback")]
        public async Task<ActionResult> ConnectCallback(
            [FromRoute] string provider,
            [FromQuery] string code,
            [FromQuery] string state,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("Missing code or state.");
            }

            if (!stateService.TryValidateState(state, provider, out var userId))
            {
                return BadRequest("Invalid or expired state.");
            }

            var result = await sender.Send(
                new ConnectExternalServiceCommand(provider, code, userId),
                cancellationToken);

            var successUrl = configuration["Integration:FrontendSuccessUrl"];
            var failedUrl = configuration["Integration:FrontendFailedUrl"];

            if (!result)
            {
                if (!string.IsNullOrWhiteSpace(failedUrl))
                {
                    return Redirect(failedUrl);
                }

                return BadRequest("Cannot connect provider with current callback data.");
            }

            if (!string.IsNullOrWhiteSpace(successUrl))
            {
                return Redirect(successUrl);
            }

            return Ok(new { connected = true, provider });
        }

        [Authorize]
        [HttpPost("save-token")]
        public async Task<ActionResult> SaveStaticToken(
            [FromBody] SaveStaticTokenRequest request,
            CancellationToken cancellationToken)
        {
            var result = await sender.Send(
                new SaveStaticTokenCommand(request.Provider, request.Token),
                cancellationToken);

            if (!result)
            {
                return BadRequest("Cannot save provider token with current request data.");
            }

            return Ok();
        }
    }
}
