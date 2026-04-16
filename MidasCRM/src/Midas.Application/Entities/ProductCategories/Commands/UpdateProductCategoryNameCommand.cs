using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.ProductCategories;

namespace Midas.Application.Entities.ProductCategories.Commands
{
    public class UpdateProductCategoryNameCommand : IRequest<ProductCategory>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
    }

    public class UpdateProductCategoryNameCommandHandler(
        IGetQueries<ProductCategory> queries,
        IEntityRepository<ProductCategory> repository)
        : IRequestHandler<UpdateProductCategoryNameCommand, ProductCategory>
    {
        public async Task<ProductCategory> Handle(UpdateProductCategoryNameCommand request, CancellationToken cancellationToken)
        {
            var productCategory = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (productCategory == null)
            {
                throw new Exception($"ProductCategory with id {request.Id} not found.");
            }

            productCategory.Update(request.Name);
            await repository.UpdateAsync(productCategory, cancellationToken);
            return productCategory;
        }
    }
}
