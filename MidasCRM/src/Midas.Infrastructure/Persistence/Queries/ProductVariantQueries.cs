using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Enums;
using Midas.Core.ProductVariants;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class ProductVariantQueries : IProductVariantQueries
    {
        private readonly ApplicationDbContext _context;
        public ProductVariantQueries(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<ProductVariant?>> GetAvailableProductVariantsAsync(ProductVariantStatus status, CancellationToken cancellationToken)
        {
            return await _context.ProductVariants
                .Where(pv => pv.Status == status && !pv.IsDeleted)
                .ToListAsync(cancellationToken);
        }
    }
}
