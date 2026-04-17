using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Products;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.Commands
{
    public class CreateProductCommand : IRequest<Product>
    {
        public required int WarehouseId { get; init; }
        public required string Name { get; init; }
        public required string Description { get; init; } 
        public required int ProductCategoryId { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
    public class CreateProductCommandHandler
        (IEntityRepository<Product> repositories, 
        IEntityRepository<Warehouse> warehouseRepositories, 
        IGetQueries<Product> productQueries,
        IGetQueries<Warehouse> warehouseQueries,
        ICurrentUserService currentUserService
        ) : IRequestHandler<CreateProductCommand, Product>
    {
        public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await warehouseQueries.GetByIdAsync(request.WarehouseId, cancellationToken);
            if (warehouse == null)
            {
                throw new Exception($"Warehouse with id {request.WarehouseId} not found");
            }

            if(warehouse.OwnerId != currentUserService.UserId)
            {
                throw new UnauthorizedAccessException("You are not the owner of this warehouse");
            }

            var category = await productQueries.GetByIdAsync(request.ProductCategoryId, cancellationToken);
            if(category == null)
            {
                throw new Exception("Category not found");
            }

            var product = Product.Create(
                request.WarehouseId,
                request.Name,
                request.Description,
                category.Id);

            await repositories.AddAsync(product, cancellationToken);
            await warehouseRepositories.UpdateAsync(warehouse, cancellationToken);
            return product;
        }
    }
}
