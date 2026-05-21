using Midas.Core.Orders;
using Midas.Core.Enums;
using System;

namespace Midas.Core.Payments
{
    public class Payment : IEntity<Guid>, ICompanyOwnedEntity
    {
        public Guid Id { get; }
        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public decimal Amount { get; private set; }
        public PaymentMethods Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid CompanyId { get; private set; }
        public bool IsDeleted { get; private set; }

        private Payment(Guid id, Guid orderId, decimal amount, PaymentMethods method, PaymentStatus status, DateTime createdAt, Guid companyId)
        {
            Id = id;
            OrderId = orderId;
            Amount = amount;
            Method = method;
            Status = status;
            CreatedAt = createdAt;
            CompanyId = companyId;
        }

        public static Payment Create(Guid orderId, decimal amount, PaymentMethods method, Guid companyId)
        {
            return new Payment(
                Guid.NewGuid(),
                orderId,
                amount,
                method,
                PaymentStatus.Pending,
                DateTime.UtcNow,
                companyId);
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
        public void UpdateStatus(PaymentStatus status)
        {
            Status = status;
        }
    }
}

