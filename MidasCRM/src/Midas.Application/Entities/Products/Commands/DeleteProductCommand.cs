using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class DeleteProductCommand : IRequest<Product>
    {
        public required int Id { get; set; }
    }
    public class DeleteProductCommandHandler
        (IGetQueries<Product, int> queries, IEntityRepository<Product> repository)
        : IRequestHandler<DeleteProductCommand, Product>
    {
        public async Task<Product> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                throw new Exception($"Product with id {request.Id} not found.");
            }
            product.MarkAsDeleted();
            await repository.UpdateAsync(product, cancellationToken);
            return product;
        }
    }
}
