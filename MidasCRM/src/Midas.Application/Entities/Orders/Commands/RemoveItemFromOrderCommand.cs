using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.Products;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Orders.Commands
{
    public class RemoveItemFromOrderCommand : ICommand<Order>
    {
        public required Guid OrderId { get; init; }
        public required int ProductId { get; init; }
        public required int OrderItemId { get; init; }
        public required int Quantity { get; init; }
        public required int ProductVariantId { get; init; }
    }
    public class RemoveItemFromOrderCommandHandler(
        IGetQueries<Order, Guid> orderQueries,
        IGetQueries<OrderItem, int> orderItemQueries,
        IGetQueries<ProductVariant, int> productVariantQueries,
        IEntityRepository<Order> orderRepository)
        : IRequestHandler<RemoveItemFromOrderCommand, Order>
    {
        public async Task<Order> Handle(RemoveItemFromOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderQueries.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Order with id {request.OrderId} not found.");
            }

            var orderItem = await orderItemQueries.GetByIdAsync(request.OrderItemId, cancellationToken);
            if(orderItem == null)
            {
                throw new Exception($"Order item with id {request.OrderItemId} not found");
            }


            var productVariant = await productVariantQueries.GetByIdAsync(request.ProductVariantId, cancellationToken);
            if(productVariant == null)
            {
                throw new Exception($"ProductVariant with id {request.ProductVariantId} not found");
            }
            productVariant.UpdateStatus(Core.Enums.ProductVariantStatus.Available);

            order.RemoveOrderItem(orderItem);

            await orderRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
