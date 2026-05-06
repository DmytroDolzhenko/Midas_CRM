using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.OrderItems
{
    public class OrderItem : IEntity<int>, IOwnedEntity
    {
        public int Id { get; }
        public Guid OrderId { get; private set; }
        public int ProductVariantId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } // Ціна за одиницю товару на момент замовлення
        public decimal CostPriceSnapshot { get; private set; } // Ціна закупівлі на момент замовлення
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }

        private OrderItem(int id, Guid orderId, int productVariantId, int quantity, decimal unitPrice, decimal costPriceSnapshot, Guid ownerId)
        {
            Id = id;
            OrderId = orderId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CostPriceSnapshot = costPriceSnapshot;
            OwnerId = ownerId;
        }

        public static OrderItem Create(
            Guid orderId,
            int productVariantId,
            int quantity,
            decimal unitPrice,
            decimal costPriceSnapshot,
            Guid ownerId)
        {
            return new OrderItem(
                0,
                orderId,
                productVariantId,
                quantity,
                unitPrice,
                costPriceSnapshot,
                ownerId);
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
        
        public void Delete()
        {
            IsDeleted = true;
        }
    }
}
