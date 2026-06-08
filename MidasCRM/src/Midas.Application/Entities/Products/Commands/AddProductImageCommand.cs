using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductImages;
using Midas.Core.Products;
using Microsoft.AspNetCore.Http;

namespace Midas.Application.Entities.Products.Commands
{
    public class AddImageToProductCommand : ICommand<ProductImage>
    {
        public required int ProductId { get; init; }
        public required IFormFile Image { get; init; }
    }

    public class AddImageToProductCommandHandler(
        IGetQueries<Product, int> productQueries,
        IFileService fileService)
        : IRequestHandler<AddImageToProductCommand, ProductImage>
    {
        public async Task<ProductImage> Handle(AddImageToProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken,
                query => query.Include(p => p.Images));

            if (product == null)
            {
                throw new Exception($"Product with id {request.ProductId} not found.");
            }

            var imageUrl = await fileService.UploadImageAsycn(request.Image, "products");

            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new Exception("Failed to upload image to Cloudinary.");
            }

            product.AddImage(imageUrl);

            var newImage = product.Images.Last();

            return newImage;
        }
    }
}
