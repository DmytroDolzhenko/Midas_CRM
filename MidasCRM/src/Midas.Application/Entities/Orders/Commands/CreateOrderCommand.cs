using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class CreateOrderCommand : ICommand<Order>
    {
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
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

            var address = Midas.Core.CustomerAddresses.CustomerAddress.Create(
                0,
                request.CustomerId,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber,
                currentUserId);

            var order = Order.Create(request.CustomerId, address, currentUserId);
            await repository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}
