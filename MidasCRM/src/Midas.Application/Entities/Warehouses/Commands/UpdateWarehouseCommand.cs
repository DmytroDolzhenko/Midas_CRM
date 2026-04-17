using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Warehouses.Commands
{
    public class UpdateWarehouseCommand : IRequest<Warehouse>
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
    }
    public class UpdateWarehouseCommandHandler(
        IGetQueries<Warehouse> warehouseQueries,
        IEntityRepository<Warehouse> warehouseRepository)
        : IRequestHandler<UpdateWarehouseCommand, Warehouse>
    {
        public async Task<Warehouse> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await warehouseQueries.GetByIdAsync(request.Id, cancellationToken);
            if (warehouse == null)
            {
                throw new Exception($"Warehouse with id {request.Id} not found.");
            }
            warehouse.Update(request.Name);
            await warehouseRepository.UpdateAsync(warehouse, cancellationToken);
            return warehouse;
        }
    }
}
