using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class CreateProductCommand : IRequest<Product>
    {
        public required string Name { get; init; }
        public required string Description { get; init; } 
        public required int ProductCategoryId { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
    public class CreateProductCommandHandler
        (IEntityRepository<Product> repositories, 
        IGetQueries<Product> getQueries
        ) : IRequestHandler<CreateProductCommand, Product>
    {
        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var category = await getQueries.GetByIdAsync(request.ProductCategoryId, cancellationToken);

            if(category == null)
            {
                throw new Exception("Category not found");
            }

            var product = Product.Create(
                request.Name,
                request.Description,
                category.Id);
            await repositories.AddAsync(product, cancellationToken);
            return product;
        }
    }
}
