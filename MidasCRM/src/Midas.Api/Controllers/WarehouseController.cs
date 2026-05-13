using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Warehouses.Commands;
using Midas.Core.Warehouses;

namespace Midas.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseController(ISender sender, IGetQueries<Warehouse, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetWarehouses(CancellationToken cancellationToken)
        {
            var warehouses = await getQueries.GetAllAsync(
                cancellationToken,
                query => query
                .Include(warehouse => warehouse.Products));
            return Ok(warehouses.Select(WarehouseDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouseById(int id, CancellationToken cancellationToken)
        {
            var warehouse = await getQueries.GetByIdAsync(id, cancellationToken);
            if (warehouse is null)
            {
                return NotFound();
            }

            return Ok(WarehouseDto.FromDomain(warehouse));
        }

        [HttpPost]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse([FromBody] CreateWarehouseDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new CreateWarehouseCommand { Name = request.Name }, cancellationToken);
            return Ok(WarehouseDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<WarehouseDto>> UpdateWarehouse(int id, [FromBody] UpdateWarehouseDto request, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new UpdateWarehouseCommand { Id = id, Name = request.Name }, cancellationToken);
            return Ok(WarehouseDto.FromDomain(result));
        }

        [HttpPost("add-product")]
        public async Task<ActionResult<WarehouseDto>> AddProductToWarehouse([FromBody] AddProductToWarehouseDto request, CancellationToken cancellationToken)
        {
            var command = new AddProductToWarehouseCommand
            {
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(WarehouseDto.FromDomain(result));
        }

        [HttpPost("remove-product")]
        public async Task<ActionResult<WarehouseDto>> RemoveProductFromWarehouse([FromBody] RemoveProductFromWarehouseDto request, CancellationToken cancellationToken)
        {
            var command = new RemoveProductFromWarehouseCommand
            {
                WarehouseId = request.WarehouseId,
                ProductId = request.ProductId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(WarehouseDto.FromDomain(result));
        }
    }
}
