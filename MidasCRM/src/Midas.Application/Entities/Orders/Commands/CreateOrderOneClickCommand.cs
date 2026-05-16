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
        public required string CustomerEmail { get; init; }
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
        IUniqCodeGenerator uniqCodeGenerator,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateOrderOneClickCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderOneClickCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var contact = Contact.Create(request.CustomerContactValue, currentUserId);

            var customer = Customer.Create(
                request.CustomerName,
                request.CustomerSurname,
                contact,
                request.CustomerEmail,
                currentUserId);

            await customerRepository.AddAsync(customer, cancellationToken);

            var address = CustomerAddress.Create(
                customer,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber,
                currentUserId);

            var uniqCode = await uniqCodeGenerator.GenerateOrderCodeAsync(
                currentUserId,
                DateTime.UtcNow,
                cancellationToken);

            var order = Order.Create(customer, address, uniqCode, currentUserId);

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
