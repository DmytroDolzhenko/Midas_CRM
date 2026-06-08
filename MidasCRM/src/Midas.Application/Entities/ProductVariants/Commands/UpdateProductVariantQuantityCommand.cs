using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class UpdateProductVariantQuantityCommand : ICommand<ProductVariant>
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateProductVariantQuantityCommandHandler(
        IGetQueries<ProductVariant, int> queries,
        IEntityRepository<ProductVariant> repository)
        : IRequestHandler<UpdateProductVariantQuantityCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(UpdateProductVariantQuantityCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (productVariant is null)
            {
                throw new Exception($"ProductVariant with id {request.Id} not found.");
            }

            productVariant.UpdateQuantity(request.Quantity);

            await repository.UpdateAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}
