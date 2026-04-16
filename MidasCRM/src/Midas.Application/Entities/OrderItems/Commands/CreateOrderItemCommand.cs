using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.OrderItems;

namespace Midas.Application.Entities.OrderItems.Commands
{
    public class CreateOrderItemCommand : IRequest<OrderItem>
    {
        public required int OrderId { get; init; }
        public required int ProductVariantId { get; init; }
        public required int Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal CostPriceSnapshot { get; init; }
    }

    public class CreateOrderItemCommandHandler(IEntityRepository<OrderItem> repository)
        : IRequestHandler<CreateOrderItemCommand, OrderItem>
    {
        public async Task<OrderItem> Handle(CreateOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = OrderItem.Create(
                request.OrderId,
                request.ProductVariantId,
                request.Quantity,
                request.UnitPrice,
                request.CostPriceSnapshot);

            await repository.AddAsync(orderItem, cancellationToken);
            return orderItem;
        }
    }
}
