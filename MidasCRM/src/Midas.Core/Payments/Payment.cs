using Midas.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Payments
{
    public class Payment : IEntity<Guid>, IOwnedEntity
    {
        public Guid Id { get;}
        public int OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethods Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }

        private Payment(Guid id, int orderId, decimal amount, PaymentMethods method, PaymentStatus status, DateTime createdAt, Guid ownerId)
        {
            Id = id;
            OrderId = orderId;
            Amount = amount;
            Method = method;
            Status = status;
            CreatedAt = createdAt;
            OwnerId = ownerId;
        }

        public static Payment Create(int orderId, decimal amount, PaymentMethods method, Guid ownerId)
        {
            return new Payment(
                Guid.NewGuid(),
                orderId,
                amount,
                method,
                PaymentStatus.Pending,
                DateTime.UtcNow,
                ownerId);
        }
        public void MasrkAsDeleted()
        {
            IsDeleted = true;
        }

    }
}
