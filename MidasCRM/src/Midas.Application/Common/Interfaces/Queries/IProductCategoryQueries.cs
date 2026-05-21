using Midas.Core.Enums;
using Midas.Core.ProductCategories;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IProductCategoryQueries
    {
        public Task<IReadOnlyList<ProductCategory>> GetAvailableCategoryAsync(CancellationToken cancellationToken);
    }
}
