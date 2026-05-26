using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.OrderItems;

namespace Midas.Application.Entities.OrderItems.Commands
{
    public class UpdateOrderItemCommand : ICommand<OrderItem>
    {
        public required int Id { get; init; }
        public required Guid OrderId { get; init; }
        public required int ProductVariantId { get; init; }
        public required int Quantity { get; init; }
        public required decimal UnitPrice { get; init; }
        public required decimal CostPriceSnapshot { get; init; }
    }

    public class UpdateOrderItemCommandHandler(
        IGetQueries<OrderItem, int> queries,
        IEntityRepository<OrderItem> repository)
        : IRequestHandler<UpdateOrderItemCommand, OrderItem>
    {
        public async Task<OrderItem> Handle(UpdateOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (orderItem == null)
            {
                throw new Exception($"OrderItem with id {request.Id} not found.");
            }

            orderItem.Update(
                request.OrderId,
                request.ProductVariantId,
                request.Quantity,
                request.UnitPrice,
                request.CostPriceSnapshot);

            await repository.UpdateAsync(orderItem, cancellationToken);
            return orderItem;
        }
    }
}
