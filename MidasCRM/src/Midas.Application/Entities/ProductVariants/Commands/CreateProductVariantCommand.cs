using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class CreateProductVariantCommand : ICommand<ProductVariant>
    {
        public required int ProductId { get; init; }
        public required string UniqCode { get; init; }
        public required string Color { get; init; }
        public required string Size { get; init; }
        public required decimal CostPrice { get; init; }
        public required decimal SellPrice { get; init; }
    }

    public class CreateProductVariantCommandHandler(IEntityRepository<ProductVariant> repository)
        : IRequestHandler<CreateProductVariantCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(CreateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var productVariant = ProductVariant.Create(
                request.ProductId,
                request.UniqCode,
                request.Color,
                request.Size,
                request.CostPrice,
                request.SellPrice);

            await repository.AddAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}
