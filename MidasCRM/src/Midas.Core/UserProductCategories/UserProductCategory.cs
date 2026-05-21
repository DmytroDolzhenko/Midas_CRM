using Midas.Core.ProductCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.UserProductCategories
{
    public class UserProductCategory
    {
        public Guid UserId { get; set; }
        public int ProductCategoryId { get; set; }

        public ProductCategory ProductCategory { get; set; } = null!;
    }
}
