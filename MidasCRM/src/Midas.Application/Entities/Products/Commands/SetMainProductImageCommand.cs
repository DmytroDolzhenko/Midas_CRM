using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductImages;
using Midas.Core.Products;

namespace Midas.Application.Entities.Products.Commands
{
    public class SetMainProductImageCommand : ICommand<ProductImage>
    {
        public required int ProductId { get; init; }
        public required int ImageId { get; init; }
    }

    public class SetMainProductImageCommandHandler(
        IGetQueries<Product, int> productQueries,
        IEntityRepository<Product> productRepository)
        : IRequestHandler<SetMainProductImageCommand, ProductImage>
    {
        public async Task<ProductImage> Handle(SetMainProductImageCommand request, CancellationToken cancellationToken)
        {
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken,
                query => query.Include(p => p.Images));

            if (product == null)
            {
                throw new Exception($"Product with id {request.ProductId} not found.");
            }

            var image = product.SetMainImage(request.ImageId);
            await productRepository.UpdateAsync(product, cancellationToken);

            return image;
        }
    }
}
