using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.ProductVariants
{
    public class ProductVariant
    {
        public int Id { get;}
        public int ProductId { get; private set; }
        public string UniqCode { get; private set; }
        public string Color { get; private set; }
        public string Size { get; private set; }
        public int StockQuantity { get; private set; }
        public decimal CostPrice { get; private set; }
        public decimal SellPrice { get; private set; }

        private ProductVariant(
            int id,
            int productId,
            string uniqCode,
            string color,
            string size,
            decimal costPrice,
            decimal sellPrice)
        {
            Id = id;
            ProductId = productId;
            UniqCode = uniqCode;
            Color = color;
            Size = size;
            CostPrice = costPrice;
            SellPrice = sellPrice;
        }

        public static ProductVariant Create(
            int productId,
            string uniqCode,
            string color,
            string size,
            decimal costPrice,
            decimal sellPrice)
        {
            return new ProductVariant(
                0,
                productId,
                uniqCode,
                color,
                size,
                costPrice,
                sellPrice);
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
    }
}
