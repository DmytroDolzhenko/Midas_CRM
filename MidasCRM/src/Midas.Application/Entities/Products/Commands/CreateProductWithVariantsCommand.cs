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
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class CreateProductWithVariantsCommand : ICommand<Product>
    {
        public string Name { get; init; } = null!;
        public string Description { get; init; } = null!;
        public decimal Weight { get; init; }
        public int WarehouseId { get; init; }
        public int ProductCategoryId { get; init; }
        public required List<IFormFile> Images { get; init; }
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
        IFileService fileService)
        : IRequestHandler<CreateProductWithVariantsCommand, Product>
    {
        public async Task<Product> Handle(CreateProductWithVariantsCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var product = Product.Create(
                request.WarehouseId,
                request.Name,
                request.Description,
                request.Weight,
                request.ProductCategoryId,
                currentUserId);

            if (request.Images != null)
            {
                foreach (var image in request.Images)
                {
                    if (image.Length == 0) continue;

                    var imageUrl = await fileService.UploadImageAsycn(image, "products");

                    if (string.IsNullOrEmpty(imageUrl))
                    {
                        throw new Exception("Failed to upload image to Cloudinary.");
                    }

                    product.AddImage(imageUrl);
                }
            }

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
                    currentUserId);

                await variantRepository.AddAsync(productVariant, cancellationToken);
            }

            await productRepository.AddAsync(product, cancellationToken);

            return product;
        }
    }
}
