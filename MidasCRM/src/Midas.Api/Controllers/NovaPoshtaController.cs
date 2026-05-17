using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.DTOs.NovaPoshta;
using Midas.Application.Entities.NovaPoshta;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NovaPoshtaController(ISender sender) : ControllerBase
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
    }
}
