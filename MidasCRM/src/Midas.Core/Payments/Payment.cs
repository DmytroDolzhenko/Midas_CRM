using Midas.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Payments
{
    public class Payment
    {
        public int Id { get;}
        public int OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethods Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private Payment(int id, int orderId, decimal amount, PaymentMethods method, PaymentStatus status, DateTime createdAt)
        {
            Id = id;
            OrderId = orderId;
            Amount = amount;
            Method = method;
            Status = status;
            CreatedAt = createdAt;
        }

        public static Payment Create(int orderId, decimal amount, PaymentMethods method)
        {
            return new Payment(
                0,
                orderId,
                amount,
                method,
                PaymentStatus.Pending,
                DateTime.UtcNow);
        }
        public void MasrkAsDeleted()
        {
            IsDeleted = true;
        }

    }
}
