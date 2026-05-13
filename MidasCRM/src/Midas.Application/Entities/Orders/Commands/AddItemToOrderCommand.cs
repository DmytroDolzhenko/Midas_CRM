using MediatR;
using Microsoft.EntityFrameworkCore;
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
    public class AddItemToOrderCommand : ICommand<Order>
    {
        public required Guid OrderId { get; init; }
        public required int ProductId { get; init; }
        public required int Quantity { get; init; }
        public required int ProductVariantId { get; init; }
    }
    public class AddItemToOrderCommandHandler(
        IGetQueries<Order, Guid> orderQueries,
        IGetQueries<ProductVariant, int> variantQueries,
        IGetQueries<Product, int> productQueries,
        IEntityRepository<Order> orderRepository)
        : IRequestHandler<AddItemToOrderCommand, Order>
    {
        public async Task<Order> Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
        {
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken,
                query => query.Include(p => p.Variants));
            if (product == null)
            {
                throw new Exception($"Product with id {request.ProductId} not found.");
            }

            var productVariant = await variantQueries.GetByIdAsync(request.ProductVariantId, cancellationToken);
            if (productVariant == null)
            {
                throw new Exception($"Product variant with id {request.ProductVariantId} not found.");
            }

            var order = await orderQueries.GetByIdAsync(request.OrderId, cancellationToken,
                query => query.Include(o => o.OrderItems));
            if (order == null)
            {
                throw new Exception($"Order with id {request.OrderId} not found.");
            }

            var existingItem = order.OrderItems
            .FirstOrDefault(oi => oi.ProductVariantId == request.ProductVariantId);

            if(existingItem != null)
            {
                var updatedQuantity = existingItem.Quantity + request.Quantity;
                existingItem.UpdateQuantity(updatedQuantity);
            }
            else
            {
                var orderItem = OrderItem.Create(
                    request.OrderId,
                    request.ProductVariantId,
                    request.Quantity,
                    productVariant.CostPrice,
                    productVariant.SellPrice,
                    order.OwnerId);
                order.AddOrderItem(orderItem);
            }

            order.RecalculateTotalCost();

            productVariant.UpdateStatus(Core.Enums.ProductVariantStatus.InOrder);

            await orderRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
