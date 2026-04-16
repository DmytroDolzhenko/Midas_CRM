using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class UpdateProductVariantCommand : IRequest<ProductVariant>
    {
        public required int Id { get; init; }
        public required int ProductId { get; init; }
        public required string UniqCode { get; init; }
        public required string Color { get; init; }
        public required string Size { get; init; }
        public required decimal CostPrice { get; init; }
        public required decimal SellPrice { get; init; }
    }

    public class UpdateProductVariantCommandHandler(
        IGetQueries<ProductVariant> queries,
        IEntityRepository<ProductVariant> repository)
        : IRequestHandler<UpdateProductVariantCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(UpdateProductVariantCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (productVariant == null)
            {
                throw new Exception($"ProductVariant with id {request.Id} not found.");
            }

            productVariant.Update(
                request.ProductId,
                request.UniqCode,
                request.Color,
                request.Size,
                request.CostPrice,
                request.SellPrice);

            await repository.UpdateAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}
