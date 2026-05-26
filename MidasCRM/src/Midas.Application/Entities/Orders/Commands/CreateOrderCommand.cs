using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Orders;

namespace Midas.Application.Entities.Orders.Commands
{
    public class CreateOrderCommand : ICommand<Order>
    {
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostDepartmentNumber { get; init; }
        public required DeliveryPointType DeliveryPointType { get; init; }
        public required string Description { get; init; }
        public required ServiceType ServiceType { get; init; }
        public required CargoType CargoType { get; init; }
        public required PaymentMethods PaymentMethods { get; init; }
    }

    public class CreateOrderCommandHandler(
        IEntityRepository<Order> repository,
        IUniqCodeGenerator uniqCodeGenerator,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateOrderCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken)
                    ?? throw new UnauthorizedAccessException();

            var address = Midas.Core.CustomerAddresses.CustomerAddress.Create(
                0,
                request.CustomerId,
                request.City,
                request.PostDepartmentNumber,
                request.DeliveryPointType,
                companyId);

            var uniqCode = await uniqCodeGenerator.GenerateOrderCodeAsync(
                companyId,
                DateTime.UtcNow,
                cancellationToken);

            var order = Order.Create(request.CustomerId, address, request.ServiceType, request.CargoType, uniqCode, companyId, request.PaymentMethods, request.Description);

            order.RecalculateTotalWeight();

            await repository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}

