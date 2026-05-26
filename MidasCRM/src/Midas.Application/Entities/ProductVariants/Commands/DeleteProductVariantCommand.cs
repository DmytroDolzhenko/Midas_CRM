using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.ProductVariants;

namespace Midas.Application.Entities.ProductVariants.Commands
{
    public class DeleteProductVariantCommand : ICommand<ProductVariant>
    {
        public required int Id { get; init; }
    }

    public class DeleteProductVariantCommandHandler(
        IGetQueries<ProductVariant, int> queries,
        IEntityRepository<ProductVariant> repository)
        : IRequestHandler<DeleteProductVariantCommand, ProductVariant>
    {
        public async Task<ProductVariant> Handle(DeleteProductVariantCommand request, CancellationToken cancellationToken)
        {
            var productVariant = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (productVariant == null)
            {
                throw new Exception($"ProductVariant with id {request.Id} not found.");
            }

            productVariant.MarkAsDeleted();
            await repository.DeleteAsync(productVariant, cancellationToken);
            return productVariant;
        }
    }
}
