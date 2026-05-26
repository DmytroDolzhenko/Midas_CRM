using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTO.NovaPoshta;
using Midas.Application.DTOs.NovaPoshta;
using Midas.Core.NovaPoshta;
using Midas.Core.UserIntegrations.Midas.Core.UserIntegrations;

namespace Midas.Application.Entities.NovaPoshta.Commands
{
    public record SaveLogisticProfileCommand(
        string SendersPhone,
        string CityName,
        string WarehouseQuery
    ) : IRequest<bool>;

    public class SaveLogisticProfileHandler(
        IApplicationDbContext context,
        INovaPoshtaClient npClient,
        ICurrentUserService currentUserService)
        : IRequestHandler<SaveLogisticProfileCommand, bool>
    {
        public async Task<bool> Handle(SaveLogisticProfileCommand request, CancellationToken ct)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(ct) ?? throw new Exception("Користувач не авторизований");

            var integration = await context.UserIntegrations
                .Include(x => x.LogisticProfile)
                .ThenInclude(lp => lp.SenderAddresses)
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Provider == "novaposhta", ct);

            if (integration == null)
                throw new Exception("Спочатку додайте API-ключ Нової Пошти");

            var cleanPhone = new string(request.SendersPhone.Where(char.IsDigit).ToArray());
            if (cleanPhone.StartsWith("0")) cleanPhone = "38" + cleanPhone;

            var counterparties = await npClient.ExecuteAsync<GetCounterpartiesRequest, NpCounterpartyItem>(
                companyId, "Counterparty", "getCounterparties", new GetCounterpartiesRequest("Sender"), ct);

            if (counterparties == null || !counterparties.Any())
                throw new Exception("Не знайдено жодного відправника для цього API-ключа в Новій Пошті.");

            var senderRef = counterparties.First().Ref;

            var contacts = await npClient.ExecuteAsync<GetContactPersonsRequest, NpContactPersonItem>(
                companyId, "Counterparty", "getCounterpartyContactPersons", new GetContactPersonsRequest(senderRef), ct);

            if (contacts == null || !contacts.Any())
                throw new Exception("У вашому акаунті Нової Пошти не знайдено контактних осіб.");

            var contactSenderRef = contacts.First().Ref;

            var city = await context.NovaPoshtaCities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Description.ToLower() == request.CityName.ToLower(), ct);

            if (city == null)
                throw new Exception($"Місто '{request.CityName}' не знайдено. Оновіть довідник НП або перевірте назву.");

            var warehouseQuery = request.WarehouseQuery.Trim().ToLower();
            var digitsOnly = new string(warehouseQuery.Where(char.IsDigit).ToArray());

            var warehouse = await context.NovaPoshtaWarehouses
                .AsNoTracking()
                .Where(x => x.CityRef == city.Ref)
                .OrderBy(x => x.Number)
                .FirstOrDefaultAsync(x =>
                    x.Number.ToLower() == warehouseQuery ||
                    (!string.IsNullOrEmpty(digitsOnly) && x.Number == digitsOnly) ||
                    x.Description.ToLower().Contains(warehouseQuery), ct);

            if (warehouse == null)
            {
                var npWarehouses = await npClient.ExecuteAsync<GetNPWarehousesProperties, NovaPoshtaWarehouseDto>(
                    companyId,
                    "Address",
                    "getWarehouses",
                    new GetNPWarehousesProperties(city.Ref),
                    ct);

                var fromApi = npWarehouses
                    .OrderBy(x => x.Number)
                    .FirstOrDefault(x =>
                        x.Number.ToLower() == warehouseQuery ||
                        (!string.IsNullOrEmpty(digitsOnly) && x.Number == digitsOnly) ||
                        x.Description.ToLower().Contains(warehouseQuery));

                if (fromApi != null)
                {
                    warehouse = NovaPoshtaWarehouse.Create(
                        fromApi.Ref,
                        city.Ref,
                        fromApi.Description,
                        fromApi.Number,
                        string.Empty,
                        string.Empty);
                }
            }

            if (warehouse == null)
                throw new Exception($"Відділення '{request.WarehouseQuery}' у місті '{request.CityName}' не знайдено.");

            var profile = integration.LogisticProfile;
            if (profile == null)
                throw new Exception("Профіль логістики не ініціалізовано.");

            profile.Update(senderRef, contactSenderRef, cleanPhone);

            var existingAddress = profile.SenderAddresses.FirstOrDefault();
            if (existingAddress != null)
            {
                existingAddress.Update(
                    city.Ref,
                    warehouse.Ref,
                    warehouse.WarehouseIndex,
                    warehouse.Description);
            }
            else
            {
                var newAddress = UserSenderAddress.Create(
                    city.Ref,
                    warehouse.Ref,
                    warehouse.WarehouseIndex,
                    warehouse.Description);
                profile.AddAddress(newAddress);
            }

            await context.SaveChangesAsync(ct);
            return true;
        }
    }
}



