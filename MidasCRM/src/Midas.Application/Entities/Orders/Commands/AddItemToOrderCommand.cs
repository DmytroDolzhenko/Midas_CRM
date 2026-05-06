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
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new Exception($"Product with id {request.ProductId} not found.");
            }

            var productVariant = await variantQueries.GetByIdAsync(request.ProductVariantId, cancellationToken);
            if (productVariant == null)
            {
                throw new Exception($"Product variant with id {request.ProductVariantId} not found.");
            }

            var order = await orderQueries.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Order with id {request.OrderId} not found.");
            }

            var orderItem = OrderItem.Create(
                request.OrderId,
                request.ProductVariantId,
                request.Quantity,
                productVariant.CostPrice,
                productVariant.SellPrice,
                order.OwnerId);

            productVariant.UpdateStatus(Core.Enums.ProductVariantStatus.InOrder);

            order.AddOrderItem(orderItem);
            await orderRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
