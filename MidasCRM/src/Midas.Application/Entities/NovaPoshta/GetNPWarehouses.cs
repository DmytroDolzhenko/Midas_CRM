using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTOs.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.NovaPoshta
{
    public record GetNPWarehousesQuery(string WarehouseRef) : IRequest<List<NovaPoshtaWarehouseDto>>;

    public class GetNPWarehousesQueryHandler : IRequestHandler<GetNPWarehousesQuery, List<NovaPoshtaWarehouseDto>>
    {
        private readonly INovaPoshtaClient _npClient;
        private readonly ICurrentUserService _currentUser;
        public GetNPWarehousesQueryHandler(INovaPoshtaClient npClient, ICurrentUserService currentUser)
        {
            _npClient = npClient;
            _currentUser = currentUser;
        }

        public async Task<List<NovaPoshtaWarehouseDto>> Handle(GetNPWarehousesQuery request, CancellationToken ct)
        {
            var companyId = await _currentUser.GetCompanyIdAsync(ct) ?? throw new UnauthorizedAccessException();

            return await _npClient.ExecuteAsync<GetNPWarehousesProperties, NovaPoshtaWarehouseDto>(
                companyId,
                "Address",
                "getWarehouses",
                new GetNPWarehousesProperties(request.WarehouseRef),
                ct);
        }
    }
}
