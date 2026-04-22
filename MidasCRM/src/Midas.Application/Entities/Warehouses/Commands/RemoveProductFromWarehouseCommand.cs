using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Products;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Warehouses.Commands
{
    public class RemoveProductFromWarehouseCommand : IRequest<Warehouse>
    {
        public required int WarehouseId { get; set; }
        public required int ProductId { get; set; }
    }

    public class RemoveProductFromWarehouseCommandHandler(
        IGetQueries<Warehouse, int> warehouseQueries,
        IGetQueries<Product, int> productQueries,
        IEntityRepository<Warehouse> warehouseRepository)
        : IRequestHandler<RemoveProductFromWarehouseCommand, Warehouse>
    {
        public async Task<Warehouse> Handle(RemoveProductFromWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await warehouseQueries.GetByIdAsync(request.WarehouseId, cancellationToken);
            if (warehouse == null)
            {
                throw new Exception($"Warehouse with id {request.WarehouseId} not found.");
            }

            var product = await productQueries.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new Exception($"Product with id {request.ProductId} not found.");
            }

            warehouse.RemoveProduct(product);
            await warehouseRepository.UpdateAsync(warehouse, cancellationToken);
            return warehouse;
        }
    }
}
