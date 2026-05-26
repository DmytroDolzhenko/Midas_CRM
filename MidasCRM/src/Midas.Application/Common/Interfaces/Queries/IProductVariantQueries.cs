using Midas.Core.Enums;
using Midas.Core.Orders;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IProductVariantQueries
    {
        public Task<IReadOnlyList<ProductVariant?>> GetAvailableProductVariantsAsync(ProductVariantStatus status, CancellationToken cancellationToken);
    }
}
