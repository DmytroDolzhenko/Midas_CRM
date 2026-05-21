using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTO.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.NovaPoshta
{
    public record GetNpSenderContactsQuery(string SenderRef) : IRequest<List<NpContactDto>>;

    public class GetNpSenderContactsHandler(
        INovaPoshtaClient npClient,
        ICurrentUserService currentUserService)
        : IRequestHandler<GetNpSenderContactsQuery, List<NpContactDto>>
    {
        public async Task<List<NpContactDto>> Handle(GetNpSenderContactsQuery request, CancellationToken ct)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(ct) ?? throw new Exception("Користувач не авторизований");

            // Запитуємо контактних осіб для конкретного контрагента відправника
            var result = await npClient.ExecuteAsync<GetContactPersonsRequest, NpContactPersonItem>(
                companyId,
                "Counterparty",
                "getCounterpartyContactPersons",
                new GetContactPersonsRequest(request.SenderRef),
                ct);

            if (result == null) return new List<NpContactDto>();

            return result.Select(x => new NpContactDto(x.Ref, x.Description, x.Phones)).ToList();
        }
    }


}

