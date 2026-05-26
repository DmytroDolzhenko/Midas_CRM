using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Enums;
using Midas.Core.Orders;
using Midas.Core.ProductVariants;
using Midas.Infrastructure.Persistence.Queries.Extensions;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class ProductVariantQueries : IProductVariantQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public ProductVariantQueries(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        private async Task<IQueryable<ProductVariant>> GetFilteredProductVariantsAsync(CancellationToken cancellationToken)
        {
            var companyId = await _currentUserService.GetCompanyIdAsync(cancellationToken);
            var userId = _currentUserService.UserId;

            return _context.ProductVariants.ApplyCompanyFilter(_context, userId, companyId);
        }
        public async Task<IReadOnlyList<ProductVariant?>> GetAvailableProductVariantsAsync(ProductVariantStatus status, CancellationToken cancellationToken)
        {
            var productVariantsQuery = await GetFilteredProductVariantsAsync(cancellationToken);

            return await productVariantsQuery
                .Where(pv => pv.Status == status && !pv.IsDeleted)
                .ToListAsync(cancellationToken);
        }
    }
}
