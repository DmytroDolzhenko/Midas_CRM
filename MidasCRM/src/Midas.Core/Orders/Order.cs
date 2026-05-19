using Midas.Core.CustomerAddresses;
using Midas.Core.Customers;
using Midas.Core.Enums;
using Midas.Core.OrderItems;
using Midas.Core.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Orders
{
    public class Order : IEntity<Guid>, IOwnedEntity
    {
        public Guid Id { get; }
        public string UniqCode { get; private set; }
        public string? TrackingNumber { get; private set; }

        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;

        public int AddressId { get; private set; }
        public CustomerAddress Address { get; private set; } = null!;
        public ServiceType ServiceType { get; private set; }

        public OrderStatus Status { get; private set; }
        public decimal TotalCost { get; private set; }
        public decimal TotalWeight { get; private set; }
        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public Guid OwnerId { get; private set; }
        public PaymentMethods PaymentMethods { get; private set; }


        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();


        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public bool IsDeleted { get; private set; }

        private Order(
            Guid id,
            string uniqCode,
            int customerId,
            int addressId,
            ServiceType serviceType,
            OrderStatus status,
            decimal totalCost,
            decimal totalWeight,
            string description,
            DateTime createdAt,
            Guid ownerId,
            PaymentMethods paymentMethods)
        {
            Id = id;
            UniqCode = uniqCode;
            CustomerId = customerId;
            AddressId = addressId;
            ServiceType = serviceType;
            Status = status;
            TotalCost = totalCost;
            TotalWeight = totalWeight;
            Description = description;
            CreatedAt = createdAt;
            OwnerId = ownerId;
            PaymentMethods = paymentMethods;
        }

        public static Order Create(int customerId, CustomerAddress address, ServiceType serviceType, string uniqCode, Guid ownerId, PaymentMethods paymentMethods, string description)
        {
            var order = new Order(
                Guid.NewGuid(),
                uniqCode,
                customerId,
                address.Id,
                serviceType,
                OrderStatus.Pending,
                0,
                0,
                description,
                DateTime.UtcNow,
                ownerId,
                paymentMethods
                );

            order.Address = address;
            return order;
        }

        public static Order Create(Customer customer, CustomerAddress address, ServiceType serviceType, string uniqCode, Guid ownerId, PaymentMethods paymentMethods, string description)
        {
            var order = new Order(
                Guid.NewGuid(),
                uniqCode,
                customer.Id,
                address.Id,
                serviceType,
                OrderStatus.Pending,
                0,
                0,
                description,
                DateTime.UtcNow,
                ownerId,
                paymentMethods);

            order.Customer = customer;
            order.Address = address;
            return order;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        public void UpdateStatus(OrderStatus newStatus)
        {
            Status = newStatus;
        }

        public void RemoveOrderItem(OrderItem orderItem)
        {
            _orderItems.Remove(orderItem);
            RecalculateTotalCost();
            RecalculateTotalWeight();
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _orderItems.Add(orderItem);
            RecalculateTotalCost();
            RecalculateTotalWeight();
        }

        public void RecalculateTotalCost()
        {
            TotalCost = 0;
            foreach (var item in _orderItems)
            {
                TotalCost += item.Quantity * item.UnitPrice;
            }
        }
        public void SetTrackingNumber(string trackingNumber)
        {
            if (string.IsNullOrWhiteSpace(trackingNumber))
                throw new ArgumentException("Tracking number cannot be empty", nameof(trackingNumber));

            TrackingNumber = trackingNumber;


            this.Status = OrderStatus.Processing;
        }
        public void ChangePaymentMethods(PaymentMethods newPaymentMethods)
        {
            PaymentMethods = newPaymentMethods;
        }
        public void RecalculateTotalWeight()
        {
            TotalWeight = 0;
            foreach (var item in _orderItems)
            {
                if (item.ProductVariant?.Product != null)
                {
                    TotalWeight += item.Quantity * item.ProductVariant.Product.Weight;
                }
            }
        }
        public void ChangeServiceType(ServiceType newServiceType)
        {
            ServiceType = newServiceType;
        }
        public void SetTotalWeight(decimal weight)
        {
            TotalWeight = weight;
        }
    }
}
