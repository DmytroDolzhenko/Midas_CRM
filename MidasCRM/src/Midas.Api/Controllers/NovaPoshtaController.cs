using MediatR;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
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
        ApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : ControllerBase
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
            try
            {
                var ttnNumber = await sender.Send(new CreateNovaPoshtaDocumentCommand(orderId), ct);
                return Ok(new { trackingNumber = ttnNumber });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("не знайдено", StringComparison.OrdinalIgnoreCase))
                {
                    return NotFound(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("sync-directories")]
        public async Task<ActionResult<object>> SyncDirectories(CancellationToken ct)
        {
            try
            {
                var activeCompanyId = await currentUserService.GetCompanyIdAsync(ct);
                if (activeCompanyId is not null)
                {
                    var hasActiveCompanyIntegration = await dbContext.UserIntegrations
                        .AnyAsync(x => x.CompanyId == activeCompanyId.Value && x.Provider == "novaposhta", ct);

                    if (hasActiveCompanyIntegration)
                    {
                        await syncService.SyncAllDataAsync(activeCompanyId.Value, ct);
                        return Ok(new { message = "Nova Poshta directories synced successfully for active company." });
                    }
                }

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
                    return BadRequest(new { message = "Не знайдено компанію для синхронізації. Перевірте активну компанію та налаштування SystemCompanyId/SystemAdminUserId." });
                }

                var hasSystemCompanyIntegration = await dbContext.UserIntegrations
                    .AnyAsync(x => x.CompanyId == systemCompanyId.Value && x.Provider == "novaposhta", ct);

                if (!hasSystemCompanyIntegration)
                {
                    return BadRequest(new { message = "Інтеграція Нової Пошти не налаштована для активної або системної компанії." });
                }

                await syncService.SyncAllDataAsync(systemCompanyId.Value, ct);
                return Ok(new { message = "Nova Poshta directories synced successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
