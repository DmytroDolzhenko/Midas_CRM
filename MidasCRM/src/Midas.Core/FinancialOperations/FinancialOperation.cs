using Midas.Core.Enums;
using Midas.Core.Orders;
using Midas.Core.Users;

namespace Midas.Core.FinancialOperations
{
    public class FinancialOperation : IEntity<Guid>, ICompanyOwnedEntity
    {
        public Guid Id { get; }
        public Guid CompanyId { get; private set; }

        public FinancialOperationType OperationType { get; private set; }
        public FinancialOperationCategory Category { get; private set; }
        public decimal Amount { get; private set; }
        public string? Comment { get; private set; }

        public Guid? OrderId { get; private set; }
        public Order? Order { get; private set; }

        public Guid? CreatedByUserId { get; private set; }
        public User? CreatedByUser { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private FinancialOperation(
            Guid id,
            Guid companyId,
            FinancialOperationType operationType,
            FinancialOperationCategory category,
            decimal amount,
            string? comment,
            Guid? orderId,
            Guid? createdByUserId,
            DateTime createdAt)
        {
            Id = id;
            CompanyId = companyId;
            OperationType = operationType;
            Category = category;
            Amount = amount;
            Comment = comment;
            OrderId = orderId;
            CreatedByUserId = createdByUserId;
            CreatedAt = createdAt;
        }

        public static FinancialOperation Create(
            Guid companyId,
            FinancialOperationType operationType,
            FinancialOperationCategory category,
            decimal amount,
            string? comment,
            Guid? orderId,
            Guid? createdByUserId)
        {
            return new FinancialOperation(
                Guid.NewGuid(),
                companyId,
                operationType,
                category,
                amount,
                comment,
                orderId,
                createdByUserId,
                DateTime.UtcNow);
        }

        public void Update(
            FinancialOperationType operationType,
            FinancialOperationCategory category,
            decimal amount,
            string? comment,
            Guid? orderId)
        {
            OperationType = operationType;
            Category = category;
            Amount = amount;
            Comment = comment;
            OrderId = orderId;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
    }
}
