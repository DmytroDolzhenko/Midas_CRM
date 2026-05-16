using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Products;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class CreateProductVariantCommand : ICommand<ProductVariant>
    {
        public required int ProductId { get; init; }
        public required string UniqCode { get; init; }
        public required string Color { get; init; }
        public required string Size { get; init; }
        public required int Quantity { get; init; }
        public required decimal CostPrice { get; init; }
        public required decimal SellPrice { get; init; }
    }

    public class CreateProductVariantCommandHandler(
        IEntityRepository<ProductVariant> repository,
        IGetQueries<Product, int> productQueries,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateProductVariantCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken);
            if (product is null)
            {
                throw new Exception($"Product with ID {request.ProductId} not found");
            }

            var productVariant = ProductVariant.Create(
                request.ProductId,
                request.UniqCode,
                request.Color,
                request.Size,
                request.Quantity,
                request.CostPrice,
                request.SellPrice,
                Core.Enums.ProductVariantStatus.Available,
                currentUserId);

            await repository.AddAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}
