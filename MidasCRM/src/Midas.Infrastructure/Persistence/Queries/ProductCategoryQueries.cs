using Infrastructure.Persistence;
using Infrastructure.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.ProductCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class ProductCategoryQueries : IProductCategoryQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public ProductCategoryQueries(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<IReadOnlyList<ProductCategory>> GetAvailableCategoryAsync(CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId
                ?? throw new UnauthorizedAccessException("Користувач не авторизований");

            return await _context.ProductCategories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Where(c => c.IsPublic || c.UserCategories.Any(uc => uc.UserId == userId))
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
