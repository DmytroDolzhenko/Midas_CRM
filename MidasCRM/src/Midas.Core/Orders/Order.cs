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

        public int CustomerId { get; private set; }
        public Customer Customer { get; private set; } = null!;

        public int AddressId { get; private set; }
        public CustomerAddress Address { get; private set; } = null!;

        public OrderStatus Status { get; private set; }
        public decimal TotalCost { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid OwnerId { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public bool IsDeleted { get; private set; }

        private Order(Guid id, string uniqCode, int customerId, int addressId, OrderStatus status, decimal totalCost, DateTime createdAt, Guid ownerId)
        {
            Id = id;
            UniqCode = uniqCode;
            CustomerId = customerId;
            AddressId = addressId;
            Status = status;
            TotalCost = totalCost;
            CreatedAt = createdAt;
            OwnerId = ownerId;
        }

        public static Order Create(int customerId, CustomerAddress address, string uniqCode, Guid ownerId)
        {
            var order = new Order(
                Guid.NewGuid(),
                uniqCode,
                customerId,
                address.Id,
                OrderStatus.Pending,
                0,
                DateTime.UtcNow,
                ownerId);

            order.Address = address;
            return order;
        }

        public static Order Create(Customer customer, CustomerAddress address, string uniqCode, Guid ownerId)
        {
            var order = new Order(
                Guid.NewGuid(),
                uniqCode,
                customer.Id,
                address.Id,
                OrderStatus.Pending,
                0,
                DateTime.UtcNow,
                ownerId);

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
        }

        public void AddOrderItem(OrderItem orderItem)
        {
            _orderItems.Add(orderItem);
            RecalculateTotalCost();
        }

        public void RecalculateTotalCost()
        {
            TotalCost = 0;
            foreach (var item in _orderItems)
            {
                TotalCost += item.Quantity * item.UnitPrice;
            }
        }
    }
}
