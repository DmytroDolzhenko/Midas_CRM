using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.ProductCategories;

namespace Midas.Application.Entities.ProductCategories.Commands
{
    public class CreateProductCategoryCommand : IRequest<ProductCategory>
    {
        public required string Name { get; init; }
    }

    public class CreateProductCategoryCommandHandler(IEntityRepository<ProductCategory> repository)
        : IRequestHandler<CreateProductCategoryCommand, ProductCategory>
    {
        public async Task<ProductCategory> Handle(CreateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var productCategory = ProductCategory.Create(0, request.Name);
            await repository.AddAsync(productCategory, cancellationToken);
            return productCategory;
        }
    }
}
