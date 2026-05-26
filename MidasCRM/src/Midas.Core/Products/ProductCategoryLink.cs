using Midas.Core.ProductCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.Products
{
    public class ProductCategoryLink
    {
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int CategoryId { get; private set; }
        public ProductCategory Category { get; private set; } = null!;
        public static ProductCategoryLink Create(int productId, int categoryId)
        {
            return new ProductCategoryLink
            {
                ProductId = productId,
                CategoryId = categoryId
            };
        }

    }
}
