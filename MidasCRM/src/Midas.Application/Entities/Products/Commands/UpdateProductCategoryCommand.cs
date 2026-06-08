using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class UpdateProductCategoryCommand : ICommand<Product>
    {
        public required int Id { get; init; }
        public required int ProductCategoryId { get; init; }
        public required int NewProductCategoryId { get; init; }

    }

    public class UpdateProductCategoryCommandHandler
        (IGetQueries<Product, int> queries, IEntityRepository<Product> repository)
        : IRequestHandler<UpdateProductCategoryCommand, Product>
    {
        public async Task<Product> Handle(UpdateProductCategoryCommand request, CancellationToken cancellationToken)
        {
            var product = await queries.GetByIdAsync(request.Id, cancellationToken,
                query => query.Include(p => p.ProductCategories));
            if (product == null)
            {
                throw new Exception($"Product with id {request.Id} not found.");
            }
            product.UpdateCategory(request.ProductCategoryId, request.NewProductCategoryId);
            await repository.UpdateAsync(product, cancellationToken);
            return product;
        }
    }
}
