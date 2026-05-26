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
    public class ChangeWarehouseCommand : ICommand<Product>
    {
        public required int ProductId { get; init; }
        public required int NewWarehouseId { get; init; }
    }
    public class ChangeWarehouseCommandHandler
        (IEntityRepository<Product> productRepository,
        IGetQueries<Product, int> getQueries)
        : IRequestHandler<ChangeWarehouseCommand, Product>
    {
        public async Task<Product> Handle(ChangeWarehouseCommand request, CancellationToken cancellationToken)
        {
            var product = await getQueries.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new Exception($"Product with ID {request.ProductId} not found.");
            }

            product.ChangeWarehouse(request.NewWarehouseId);
            await productRepository.UpdateAsync(product, cancellationToken);
            // можливо тут треба буде ще оновлення Warehouse(warehouseRepository.UpdateAsync)
            return product;
        }
    }
}
