using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTO.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.NovaPoshta
{
    public record GetNpSendersQuery : IRequest<List<NpSenderDto>>;

    public class GetNpSendersHandler(
        INovaPoshtaClient npClient,
        ICurrentUserService currentUserService)
        : IRequestHandler<GetNpSendersQuery, List<NpSenderDto>>
    {
        public async Task<List<NpSenderDto>> Handle(GetNpSendersQuery request, CancellationToken ct)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(ct) ?? throw new Exception("Користувач не авторизований");

            // Запитуємо контрагентів з типом "Sender" (Відправник)
            var result = await npClient.ExecuteAsync<GetCounterpartiesRequest, NpCounterpartyItem>(
                companyId,
                "Counterparty",
                "getCounterparties",
                new GetCounterpartiesRequest("Sender"),
                ct);

            if (result == null) return new List<NpSenderDto>();

            return result.Select(x => new NpSenderDto(x.Ref, x.Description, x.MarketplaceName)).ToList();
        }
    }
}

