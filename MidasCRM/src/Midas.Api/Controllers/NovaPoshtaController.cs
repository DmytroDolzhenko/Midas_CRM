using MediatR;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.DTOs.NovaPoshta;
using Midas.Application.Entities.NovaPoshta;
using Midas.Application.Entities.NovaPoshta.Commands;
using Midas.Infrastructure.Persistence.Services.NovaPoshta;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NovaPoshtaController(
        ISender sender,
        NovaPoshtaSyncService syncService,
        IConfiguration configuration,
        ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpGet("cities")]
        public async Task<ActionResult<List<NovaPoshtaCityDto>>> GetCities([FromQuery] string search, CancellationToken ct)
        {
            var result = await sender.Send(new GetNPCitiesQuery(search), ct);
            return Ok(result);
        }

        [HttpGet("warehouses/{cityRef}")]
        public async Task<ActionResult<List<NovaPoshtaWarehouseDto>>> GetWarehouses([FromRoute] string cityRef, CancellationToken ct)
        {
            var result = await sender.Send(new GetNPWarehousesQuery(cityRef), ct);
            return Ok(result);
        }

        [HttpPost("documents/{orderId:guid}")]
        public async Task<ActionResult<object>> CreateDocument([FromRoute] Guid orderId, CancellationToken ct)
        {
            var ttnNumber = await sender.Send(new CreateNovaPoshtaDocumentCommand(orderId), ct);
            return Ok(new { trackingNumber = ttnNumber });
        }

        [HttpPost("sync-directories")]
        public async Task<ActionResult<object>> SyncDirectories(CancellationToken ct)
        {
            Guid? systemCompanyId = null;

            var systemCompanyIdString = configuration["NovaPoshtaSettings:SystemCompanyId"];
            if (!string.IsNullOrWhiteSpace(systemCompanyIdString)
                && Guid.TryParse(systemCompanyIdString, out var parsedCompanyId))
            {
                systemCompanyId = parsedCompanyId;
            }
            else
            {
                var adminUserIdString = configuration["NovaPoshtaSettings:SystemAdminUserId"];
                if (!string.IsNullOrWhiteSpace(adminUserIdString)
                    && Guid.TryParse(adminUserIdString, out var adminUserId))
                {
                    systemCompanyId = await dbContext.CompanyMembers
                        .Where(x => x.UserId == adminUserId)
                        .Select(x => (Guid?)x.CompanyId)
                        .FirstOrDefaultAsync(ct);
                }
            }

            if (systemCompanyId is null)
            {
                return BadRequest("System company ID is missing/invalid and cannot be resolved by SystemAdminUserId.");
            }

            await syncService.SyncAllDataAsync(systemCompanyId.Value, ct);
            return Ok(new { message = "Nova Poshta directories synced successfully." });
        }
    }
}
