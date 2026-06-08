using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class UpdateOrderDetailsCommand : ICommand<Order>
    {
        public required Guid Id { get; init; }
        public required string City { get; init; }
        public required int PostDepartmentNumber { get; init; }
        public required DeliveryPointType DeliveryPointType { get; init; }
        public required ServiceType ServiceType { get; init; }
        public required CargoType CargoType { get; init; }
        public required PaymentMethods PaymentMethods { get; init; }
        public required string Description { get; init; }
    }

    public class UpdateOrderDetailsCommandHandler(
        IGetQueries<Order, Guid> orderQueries,
        IEntityRepository<Order> orderRepository)
        : IRequestHandler<UpdateOrderDetailsCommand, Order>
    {
        public async Task<Order> Handle(UpdateOrderDetailsCommand request, CancellationToken cancellationToken)
        {
            var order = await orderQueries.GetByIdAsync(
                request.Id,
                cancellationToken,
                query => query.Include(item => item.Address));

            if (order is null)
            {
                throw new Exception($"Order with id {request.Id} not found.");
            }

            if (order.TrackingNumber is not null)
            {
                throw new InvalidOperationException("Замовлення з ТТН не можна редагувати після створення в кабінеті Нової Пошти.");
            }

            order.UpdateDetails(
                request.ServiceType,
                request.CargoType,
                request.PaymentMethods,
                request.Description);

            order.Address.UpdateDelivery(
                request.City,
                request.PostDepartmentNumber,
                request.DeliveryPointType);

            await orderRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
