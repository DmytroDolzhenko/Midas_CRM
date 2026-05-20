using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;
using Midas.Core.CustomerAddresses;
using Midas.Core.Customers;
using Midas.Core.Enums;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.Payments;
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
        public required ServiceType ServiceType { get; init; }
        public required CargoType CargoType { get; init; }
        public required string Description { get; init; }
        //public string NovaPoshtaCityRef { get; init; }
        //public string NovaPoshtaWarehouseRef { get; init; }

        public required PaymentMethods PaymentMethods { get; init; }
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
        ICustomerQueries customerQueries,
        IUniqCodeGenerator uniqCodeGenerator,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateOrderOneClickCommand, Order>
    {
        public async Task<Order> Handle(CreateOrderOneClickCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            decimal calculatedTotalWeight = 0;

            var existingCustomer = await customerQueries.GetCustomerByEmailAsync(request.CustomerEmail, cancellationToken);

            Customer customer;

            if (existingCustomer != null)
            {
                customer = existingCustomer;
            }
            else
            {
                var contact = Contact.Create(request.CustomerContactValue, currentUserId);

                customer = Customer.Create(
                    request.CustomerName,
                    request.CustomerSurname,
                    contact,
                    request.CustomerEmail,
                    currentUserId);

                await customerRepository.AddAsync(customer, cancellationToken);
            }

            var address = CustomerAddress.Create(
                customer,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber,
                currentUserId);

            //address.SetNovaPoshtaRefs(request.NovaPoshtaCityRef, request.NovaPoshtaWarehouseRef);

            var uniqCode = await uniqCodeGenerator.GenerateOrderCodeAsync(
                currentUserId,
                DateTime.UtcNow,
                cancellationToken);

            var order = Order.Create(customer, address, request.ServiceType, request.CargoType, uniqCode, currentUserId, request.PaymentMethods, request.Description);

            foreach (var item in request.Items)
            {
                var productVariant = await productVariantQueries.GetByIdAsync(item.ProductVariantId, cancellationToken,
                    query => query.Include(pv => pv.Product));
                if (productVariant is null)
                {
                    throw new Exception($"Product variant with id {item.ProductVariantId} not found.");
                }

                calculatedTotalWeight += item.Quantity * productVariant.Product.Weight;

                productVariant.UpdateStatus(ProductVariantStatus.InOrder);

                var orderItem = OrderItem.Create(
                    order.Id,
                    item.ProductVariantId,
                    item.Quantity,
                    productVariant.CostPrice,
                    productVariant.SellPrice,
                    currentUserId);

                order.AddOrderItem(orderItem);
            }

            var payment = Payment.Create(order.Id, order.TotalCost, request.PaymentMethods, currentUserId);

            //order.RecalculateTotalWeight();
            order.SetTotalWeight(calculatedTotalWeight);

            await orderRepository.AddAsync(order, cancellationToken);
            return order;
        }
    }
}
