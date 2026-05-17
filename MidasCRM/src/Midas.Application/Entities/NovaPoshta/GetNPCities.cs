using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTOs.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.NovaPoshta
{
    public record GetNPCitiesQuery(string SearchTerm) : IRequest<List<NovaPoshtaCityDto>>;

    public class GetNPCitiesQueryHandler : IRequestHandler<GetNPCitiesQuery, List<NovaPoshtaCityDto>>
    {
        private readonly INovaPoshtaClient _npClient;
        private readonly ICurrentUserService _currentUser;

        public GetNPCitiesQueryHandler(INovaPoshtaClient npClient, ICurrentUserService currentUser)
        {
            _npClient = npClient;
            _currentUser = currentUser;
        }

        public async Task<List<NovaPoshtaCityDto>> Handle(GetNPCitiesQuery request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            return await _npClient.ExecuteAsync<GetNPCitiesProperties, NovaPoshtaCityDto>(
                userId,
                "Address",
                "getCities",
                new GetNPCitiesProperties(request.SearchTerm),
                ct);
        }
    }
}
