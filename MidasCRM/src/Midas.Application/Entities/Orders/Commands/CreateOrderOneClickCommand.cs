using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;
using Midas.Core.CustomerAddresses;
using Midas.Core.Customers;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.Orders.Commands
{
    public class CreateOrderOneClickCommand : ICommand<Order>
    {
        public required string CustomerName { get; init; }
        public required string CustomerSurname { get; init; }
        public required string CustomerContactValue { get; init; }
        public required int CustomerEmail { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
        public required IReadOnlyCollection<CreateOrderOneClickCommandItem> Items { get; init; }
    }

    public class CreateOrderOneClickCommandItem
    {
        public required int ProductVariantId { get; init; }
        public required int Quantity { get; init; }
    }

    public class CreateOrderOneClickCommandHandler(
        IEntityRepository<Customer> customerRepository,
        IEntityRepository<Order> orderRepository,
        IGetQueries<ProductVariant, int> productVariantQueries,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateOrderOneClickCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderOneClickCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var customer = Customer.Create(
                request.CustomerName,
                request.CustomerSurname,
                Contact.Create(request.CustomerContactValue, currentUserId),
                request.CustomerEmail,
                currentUserId);

            await customerRepository.AddAsync(customer, cancellationToken);

            var address = CustomerAddress.Create(
                0,
                customer.Id,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber,
                currentUserId);

            var order = Order.Create(customer.Id, address, currentUserId);

            foreach (var item in request.Items)
            {
                var productVariant = await productVariantQueries.GetByIdAsync(item.ProductVariantId, cancellationToken);
                if (productVariant is null)
                {
                    throw new Exception($"Product variant with id {item.ProductVariantId} not found.");
                }

                var orderItem = OrderItem.Create(
                    order.Id,
                    item.ProductVariantId,
                    item.Quantity,
                    productVariant.CostPrice,
                    productVariant.SellPrice,
                    currentUserId);

                order.AddOrderItem(orderItem);
            }

            await orderRepository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}
