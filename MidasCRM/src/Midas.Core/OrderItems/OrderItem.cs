using Midas.Core.Orders;
using Midas.Core.ProductVariants;
using System;

namespace Midas.Core.OrderItems
{
    public class OrderItem : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }

        public Guid OrderId { get; private set; }
        public Order Order { get; private set; } = null!;

        public int ProductVariantId { get; private set; }
        public ProductVariant ProductVariant { get; private set; } = null!;

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } //закуп
        public decimal CostPriceSnapshot { get; private set; } //продаж
        public Guid CompanyId { get; private set; }
        public bool IsDeleted { get; private set; }


        private OrderItem(int id, Guid orderId, int productVariantId, int quantity, decimal unitPrice, decimal costPriceSnapshot, Guid companyId)
        {
            Id = id;
            OrderId = orderId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CostPriceSnapshot = costPriceSnapshot;
            CompanyId = companyId;
        }

        public static OrderItem Create(
            Guid orderId,
            int productVariantId,
            int quantity,
            decimal unitPrice,
            decimal costPriceSnapshot,
            Guid companyId)
        {
            return new OrderItem(
                0,
                orderId,
                productVariantId,
                quantity,
                unitPrice,
                costPriceSnapshot,
                companyId);
        }

        public void Update(
            Guid orderId,
            int productVariantId,
            int quantity,
            decimal unitPrice,
            decimal costPriceSnapshot)
        {
            OrderId = orderId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CostPriceSnapshot = costPriceSnapshot;
        }
        public void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
        }
        public void Delete()
        {
            IsDeleted = true;
        }
    }
}

