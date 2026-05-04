using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class DeleteOrderCommand : ICommand<Order>
    {
        public required Guid Id { get; init; }
    }

    public class DeleteOrderCommandHandler(
        IGetQueries<Order, Guid> queries,
        IEntityRepository<Order> repository)
        : IRequestHandler<DeleteOrderCommand, Order>
    {
        public async Task<Order> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (order == null)
            {
                throw new Exception($"Order with id {request.Id} not found.");
            }

            order.MarkAsDeleted();
            await repository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
