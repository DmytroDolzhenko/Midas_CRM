using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAdresses;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public required int CustomerId { get; init; }
        public required CustomerAdress Adress { get; init; }
    }

    public class CreateOrderCommandHandler(
        IEntityRepository<Order> repository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateOrderCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId
                    ?? throw new UnauthorizedAccessException();

            var order = Order.Create(request.CustomerId, request.Adress, currentUserId);
            await repository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}
