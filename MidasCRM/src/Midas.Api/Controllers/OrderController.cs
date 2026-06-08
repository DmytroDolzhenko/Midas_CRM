using Api.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Entities.Orders.Commands;
using Midas.Core.Orders;
using Midas.Core.Enums;

namespace Midas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController(
        ISender sender,
        IGetQueries<Order, Guid> getQueries,
        IOrderQueries orderQueries) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders(CancellationToken cancellationToken)
        {
            var orders = await getQueries.GetAllAsync(
                cancellationToken,
                query => query
                    .Include(order => order.Address)
                    .Include(order => order.OrderItems));

            return Ok(orders.Select(OrderDto.FromDomain));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(Guid id, CancellationToken cancellationToken)
        {
            var order = await getQueries.GetByIdAsync(
                id,
                cancellationToken,
                query => query
                    .Include(item => item.Address)
                    .Include(item => item.OrderItems));

            if (order is null)
            {
                return NotFound();
            }

            return Ok(OrderDto.FromDomain(order));
        }

        [HttpGet("by-customer/{customerId:int}")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersByCustomer(int customerId, CancellationToken cancellationToken)
        {
            var orders = await orderQueries.GetOrderByCustomerAsync(customerId, cancellationToken);

            return Ok(orders.Select(o => OrderDto.FromDomain(o!)).ToList());
        }

        [HttpGet("by-status/{orderStatus:int}")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersByStatus(OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            var orders = await orderQueries.GetOrderByStatusAsync(orderStatus, cancellationToken);
            return Ok(orders.Select(o => OrderDto.FromDomain(o!)).ToList());
        }

        [HttpGet("by-uniqCode/{uniqCode}")]
        public async Task<ActionResult<OrderDto>> GetOrderByUniqCodeAsync(string uniqCode, CancellationToken cancellationToken)
        {
            var order = await orderQueries.GetOrderByUniqCodeAsync(uniqCode, cancellationToken);
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
                ServiceType = request.ServiceType,
                CargoType = request.CargoType,
                PostDepartmentNumber = request.Address.PostDepartmentNumber,
                DeliveryPointType = request.Address.DeliveryPointType,
                PaymentMethods = request.PaymentMethods,
                Description = request.Description
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

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<OrderDto>> UpdateOrder(Guid id, [FromBody] UpdateOrderDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderDetailsCommand
            {
                Id = id,
                City = request.Address.City,
                PostDepartmentNumber = request.Address.PostDepartmentNumber,
                DeliveryPointType = request.Address.DeliveryPointType,
                ServiceType = request.ServiceType,
                CargoType = request.CargoType,
                PaymentMethods = request.PaymentMethods,
                Description = request.Description
            };

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
                ServiceType= request.ServiceType,
                CargoType = request.CargoType,
                Description = request.Description,
                PostalCode = request.Address.PostalCode,
                PostDepartmentNumber = request.Address.PostDepartmentNumber,
                DeliveryPointType = request.Address.DeliveryPointType,
                PaymentMethods = request.PaymentMethods,

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

        [HttpPatch("update-status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus([FromBody] UpdateOrderStatusDto request, CancellationToken cancellationToken)
        {
            var command = new UpdateOrderStatusCommand
            {
                OrderId = request.OrderId,
                Status = request.Status
            };
            var result = await sender.Send(command, cancellationToken);
            return Ok(OrderDto.FromDomain(result));
        }
    }
}
