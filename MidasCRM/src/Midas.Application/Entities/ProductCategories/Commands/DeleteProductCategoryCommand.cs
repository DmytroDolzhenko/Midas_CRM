using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.ProductCategories;

namespace Midas.Application.Entities.ProductCategories.Commands
{
    public class DeleteProductCategoryCommand : IRequest<ProductCategory>
    {
        public required int Id { get; init; }
    }

    public class DeleteProductCategoryCommandHandler(
        IGetQueries<ProductCategory> queries,
        IEntityRepository<ProductCategory> repository)
        : IRequestHandler<DeleteProductCategoryCommand, ProductCategory>
    {
        public async Task<ProductCategory> Handle(DeleteProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var productCategory = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (productCategory == null)
            {
                throw new Exception($"ProductCategory with id {request.Id} not found.");
            }

            productCategory.MarkAsDeleted();
            await repository.UpdateAsync(productCategory, cancellationToken);
            return productCategory;
        }
    }
}
