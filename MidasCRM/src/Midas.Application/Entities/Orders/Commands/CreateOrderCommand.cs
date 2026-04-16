using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public required int CustomerId { get; init; }
    }

    public class CreateOrderCommandHandler(IEntityRepository<Order> repository)
        : IRequestHandler<CreateOrderCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = Order.Create(request.CustomerId);
            await repository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}
