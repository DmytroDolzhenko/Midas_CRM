using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.OrderItems;

namespace Midas.Application.Entities.OrderItems.Commands
{
    public class DeleteOrderItemCommand : ICommand<OrderItem>
    {
        public required int Id { get; init; }
    }

    public class DeleteOrderItemCommandHandler(
        IGetQueries<OrderItem, int> queries,
        IEntityRepository<OrderItem> repository)
        : IRequestHandler<DeleteOrderItemCommand, OrderItem>
    {
        public async Task<OrderItem> Handle(DeleteOrderItemCommand request, CancellationToken cancellationToken)
        {
            var orderItem = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (orderItem == null)
            {
                throw new Exception($"OrderItem with id {request.Id} not found.");
            }

            orderItem.Delete();
            await repository.DeleteAsync(orderItem, cancellationToken);
            return orderItem;
        }
    }
}
