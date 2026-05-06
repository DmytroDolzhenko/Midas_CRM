using Midas.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.ProductVariants
{
    public class ProductVariant : IEntity<int>, IOwnedEntity
    {
        public int Id { get;}
        public int ProductId { get; private set; }
        public string UniqCode { get; private set; }
        public string Color { get; private set; }
        public string Size { get; private set; }
        public int StockQuantity { get; private set; }
        public decimal CostPrice { get; private set; }
        public decimal SellPrice { get; private set; }
        public ProductVariantStatus Status { get; private set; }
        public Guid OwnerId { get; private set; }
        public bool IsDeleted { get; private set; }

        private ProductVariant(
            int id,
            int productId,
            string uniqCode,
            string color,
            string size,
            decimal costPrice,
            decimal sellPrice,
            ProductVariantStatus status,
            Guid ownerId
            )
        {
            Id = id;
            ProductId = productId;
            UniqCode = uniqCode;
            Color = color;
            Size = size;
            CostPrice = costPrice;
            SellPrice = sellPrice;
            Status = status;
            OwnerId = ownerId;
        }

        public static ProductVariant Create(
            int productId,
            string uniqCode,
            string color,
            string size,
            decimal costPrice,
            decimal sellPrice,
            ProductVariantStatus status,
            Guid ownerId)
        {
            return new ProductVariant(
                0,
                productId,
                uniqCode,
                color,
                size,
                costPrice,
                sellPrice,
                ProductVariantStatus.Available,
                ownerId
                );
        }

        public void Update(
            int productId,
            string uniqCode,
            string color,
            string size,
            decimal costPrice,
            decimal sellPrice)
        {
            ProductId = productId;
            UniqCode = uniqCode;
            Color = color;
            Size = size;
            CostPrice = costPrice;
            SellPrice = sellPrice;
        }
        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
        public void UpdateStatus(ProductVariantStatus status)
        {
            Status = status;
        }
    }
}
