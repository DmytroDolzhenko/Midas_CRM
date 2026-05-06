using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.OrderItems.Commands;
using Midas.Core.OrderItems;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderItemController(ISender sender, IGetQueries<OrderItem, int> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderItemDto>>> GetOrderItems(CancellationToken cancellationToken)
        {
            var items = await getQueries.GetAllAsync(cancellationToken);
            return Ok(items.Select(OrderItemDto.FromDomain));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderItemDto>> GetOrderItemById(int id, CancellationToken cancellationToken)
        {
            var item = await getQueries.GetByIdAsync(id, cancellationToken);
            if (item is null)
            {
                return NotFound();
            }

            return Ok(OrderItemDto.FromDomain(item));
        }

        [HttpPost]
        public async Task<ActionResult<OrderItemDto>> CreateOrderItem([FromBody] CreateOrderItemDto request, CancellationToken cancellationToken)
        {
            var command = new CreateOrderItemCommand
            {
                OrderId = request.OrderId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                CostPriceSnapshot = request.CostPriceSnapshot
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderItemDto.FromDomain(result));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(int id, [FromBody] UpdateOrderItemDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderItemCommand
            {
                Id = id,
                OrderId = request.OrderId,
                ProductVariantId = request.ProductVariantId,
                Quantity = request.Quantity,
                UnitPrice = request.UnitPrice,
                CostPriceSnapshot = request.CostPriceSnapshot
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderItemDto.FromDomain(result));
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<OrderItemDto>> DeleteOrderItem(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderItemCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderItemDto.FromDomain(result));
        }
    }
}
