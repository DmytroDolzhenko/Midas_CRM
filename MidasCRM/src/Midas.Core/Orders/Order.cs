using Midas.Core.Enums;
using Midas.Core.OrderItems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Orders
{
    public class Order
    {
        public int Id { get;}
        public string UniqCode { get; private set; }
        public int CustomerId { get; private set; }
        public OrderStatus Status { get; private set; }
        public decimal TotalCost { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<OrderItem> _orderItems = new();
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public bool IsDeleted { get; private set; }

        private Order(int id, string uniqCode, int customerId, OrderStatus status, decimal totalCost, DateTime createdAt)
        {
            Id = id;
            UniqCode = uniqCode;
            CustomerId = customerId;
            Status = status;
            TotalCost = totalCost;
            CreatedAt = createdAt;
        }

        public static Order Create(int customerId)
        {
            return new Order(
                0,
                Guid.NewGuid().ToString(),
                customerId,
                OrderStatus.Pending,
                0,
                DateTime.UtcNow);
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

        private void RecalculateTotalCost()
        {
            TotalCost = 0;
            foreach (var item in _orderItems)
            {
                TotalCost += item.Quantity * item.UnitPrice;
            }
        }
    }
}
