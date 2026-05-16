using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Orders.Commands
{
    public class UpdateOrderStatusCommand : ICommand<Order>
    {
        public required Guid OrderId { get; init; }
        public required OrderStatus Status { get; init; }
    }
    public class UpdateOrderStatusCommandHandler(
        IGetQueries<Order, Guid> getQueries,
        IEntityRepository<Order> entityRepository,
        ICurrentUserService currentUserService) : IRequestHandler<UpdateOrderStatusCommand, Order>
    {
        public async Task<Order> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var order = await getQueries.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new Exception($"Order with ID {request.OrderId} not found.");
            }

            order.UpdateStatus(request.Status);
            await entityRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
