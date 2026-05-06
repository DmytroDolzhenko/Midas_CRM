using Api.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Orders.Commands;
using Midas.Core.Orders;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(ISender sender, IGetQueries<Order, Guid> getQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(CancellationToken cancellationToken)
        {
            var orders = await getQueries.GetAllAsync(cancellationToken);
            return Ok(orders.Select(OrderDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            var order = await getQueries.GetByIdAsync(id, cancellationToken);
            if (order is null)
            {
                return NotFound();
            }

            return Ok(OrderDto.FromDomain(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
        {
            var command = new CreateOrderCommand
            {
                CustomerId = request.CustomerId,
                City = request.Address.City,
                PostalCode = request.Address.PostalCode,
                PostDepartmentNumber = request.Address.PostDepartmentNumber
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<OrderDto>> DeleteOrder(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteOrderCommand { Id = id };
            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }

        [HttpPost("one-click")]
        public async Task<ActionResult<OrderDto>> CreateOrderOneClick(
            [FromBody] CreateOrderOneClickDto request,
            CancellationToken cancellationToken)
        {
            var command = new CreateOrderOneClickCommand
            {
                CustomerName = request.Customer.Name,
                CustomerSurname = request.Customer.Surname,
                CustomerContactValue = request.Customer.ContactValue,
                CustomerEmail = request.Customer.Email,
                City = request.Address.City,
                PostalCode = request.Address.PostalCode,
                PostDepartmentNumber = request.Address.PostDepartmentNumber,
                Items = request.Items
                    .Select(x => new CreateOrderOneClickCommandItem
                    {
                        ProductVariantId = x.ProductVariantId,
                        Quantity = x.Quantity
                    })
                    .ToList()
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }

        [HttpPost("add-item")]
        public async Task<ActionResult<OrderDto>> AddItemToOrder([FromBody] AddItemToOrderDto request, CancellationToken cancellationToken)
        {
            var command = new AddItemToOrderCommand
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                ProductVariantId = request.ProductVariantId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }

        [HttpPost("remove-item")]
        public async Task<ActionResult<OrderDto>> RemoveItemFromOrder([FromBody] RemoveItemFromOrderDto request, CancellationToken cancellationToken)
        {
            var command = new RemoveItemFromOrderCommand
            {
                OrderId = request.OrderId,
                ProductId = request.ProductId,
                OrderItemId = request.OrderItemId,
                Quantity = request.Quantity,
                ProductVariantId = request.ProductVariantId
            };

            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }
    }
}
