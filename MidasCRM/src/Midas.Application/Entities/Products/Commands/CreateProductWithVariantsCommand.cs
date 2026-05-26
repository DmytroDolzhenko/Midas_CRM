using MediatR;
using Microsoft.AspNetCore.Http;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Products;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;

namespace Midas.Application.Entities.Products.Commands
{
    public class CreateProductWithVariantsCommand : ICommand<Product>
    {
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public decimal Weight { get; init; }
        public int WarehouseId { get; init; }
        public List<int> ProductCategoryIds { get; init; } = new();
        public List<CreateVariantCommandItem> Variants { get; init; } = new();
    }
    public class CreateVariantCommandItem
    {
        public int ProductId { get; init; }
        public string Color { get; init; } = null!;
        public string Size { get; init; } = null!;
        public int Quantity { get; init; }
        public decimal CostPrice { get; init; }
        public decimal SellPrice { get; init; }
    }

    public class CreateProductWithVariantsCommandHandler(
        IEntityRepository<Product> productRepository,
        IEntityRepository<ProductVariant> variantRepository,
        IUniqCodeGenerator uniqCodeGenerator,
        ICurrentUserService currentUserService,
        IApplicationDbContext context)
        : IRequestHandler<CreateProductWithVariantsCommand, Product>
    {
        public async Task<Product> Handle(CreateProductWithVariantsCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();

            var product = Product.Create(
                request.WarehouseId,
                request.Name,
                request.Description,
                request.Weight,
                request.ProductCategoryIds,
                companyId);

            await productRepository.AddAsync(product, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            foreach (var variant in request.Variants)
            {
                var productVariant = ProductVariant.Create(
                    product.Id,
                    variant.Color,
                    variant.Size,
                    variant.Quantity,
                    variant.CostPrice,
                    variant.SellPrice,
                    ProductVariantStatus.Available,
                    await uniqCodeGenerator.GenerateProductVariantCodeAsync(product, variant.Size, variant.Color, cancellationToken),
                    companyId);

                await variantRepository.AddAsync(productVariant, cancellationToken);
            }

            //await productRepository.AddAsync(product, cancellationToken);

            return product;
        }
    }
}

