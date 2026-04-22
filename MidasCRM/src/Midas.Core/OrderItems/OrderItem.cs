using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.OrderItems
{
    public class OrderItem : IEntity<int>
    {
        public int Id { get; }
        public Guid OrderId { get; private set; }
        public int ProductVariantId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; } // Ціна за одиницю товару на момент замовлення
        public decimal CostPriceSnapshot { get; private set; } // Ціна закупівлі на момент замовлення
        public bool IsDeleted { get; private set; }

        private OrderItem(int id, Guid orderId, int productVariantId, int quantity, decimal unitPrice, decimal costPriceSnapshot)
        {
            Id = id;
            OrderId = orderId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            CostPriceSnapshot = costPriceSnapshot;
        }

        public static OrderItem Create(
            Guid orderId,
            int productVariantId,
            int quantity,
            decimal unitPrice,
            decimal costPriceSnapshot)
        {
            return new OrderItem(
                0,
                orderId,
                productVariantId,
                quantity,
                unitPrice,
                costPriceSnapshot);
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
