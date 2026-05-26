using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class UpdateProductCommand : ICommand<Product>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; }
        public required decimal Weight { get; init; }
    }
    public class UpdateProductCommandHandler
        (IGetQueries<Product, int> queries, IEntityRepository<Product> repository)
        : IRequestHandler<UpdateProductCommand, Product>
    {
        public async Task<Product> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product =await queries.GetByIdAsync(request.Id, cancellationToken);
            if (product == null)
            {
                throw new Exception($"Product with id {request.Id} not found.");
            }

            product.Update(request.Name, request.Description, request.Weight);
            await repository.UpdateAsync(product, cancellationToken);
            return product;
        }
    }
}
