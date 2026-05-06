using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Warehouses.Commands
{
    public class CreateWarehouseCommand : ICommand<Warehouse>
    {
        public required string Name { get; set; }
    }
    public class CreateWarehouseCommandHandler(
        IEntityRepository<Warehouse> warehouseRepository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateWarehouseCommand, Warehouse>
    {
        public async Task<Warehouse> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User context is missing.");

            var warehouse = Warehouse.Create(request.Name, currentUserId);
            await warehouseRepository.AddAsync(warehouse, cancellationToken);
            return warehouse;
        }
    }
}

