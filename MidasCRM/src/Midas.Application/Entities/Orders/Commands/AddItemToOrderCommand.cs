using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.Products;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Orders.Commands
{
    public class AddItemToOrderCommand : IRequest<Order>
    {
        public required int OrderId { get; init; }
        public required int ProductId { get; init; }
        public required int Quantity { get; init; }
        public required int ProductVariantId { get; init; }
    }
    public class AddItemToOrderCommandHandler(
        IGetQueries<Order> orderQueries,
        IGetQueries<ProductVariant> variantQueries,
        IGetQueries<Product> productQueries,
        IEntityRepository<Order> orderRepository)
        : IRequestHandler<AddItemToOrderCommand, Order>
    {
        public async Task<Order> Handle(AddItemToOrderCommand request, CancellationToken cancellationToken)
        {
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken);
            var productVariant = await variantQueries.GetByIdAsync(request.ProductVariantId, cancellationToken);
            var orderItem = OrderItem.Create(request.OrderId, request.ProductVariantId, request.Quantity, productVariant.CostPrice, productVariant.SellPrice);

            var order = await orderQueries.GetByIdAsync(request.OrderId, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Order with id {request.OrderId} not found.");
            }

            order.AddOrderItem(orderItem);
            await orderRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
