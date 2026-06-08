using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.DTOs.NovaPoshta;
using Midas.Application.DTOs.NovaPoshta.Requests;
using Midas.Core.NovaPoshta;
using Midas.Core.Enums;
using Midas.Core.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Midas.Application.DTO.NovaPoshta.Responses;

namespace Midas.Application.Entities.NovaPoshta.Commands
{
    public record CreateNovaPoshtaDocumentCommand(Guid OrderId) : IRequest<string?>;
    public class NpContactPersonItem
    {
        public string Ref { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class CreateNovaPoshtaDocumentHandler(
        IApplicationDbContext context,
        INovaPoshtaClient npClient,
        ICurrentUserService currentUserService,
        IGetQueries<Order, Guid> orderQuery)
        : IRequestHandler<CreateNovaPoshtaDocumentCommand, string?>
    {
        public async Task<string?> Handle(CreateNovaPoshtaDocumentCommand request, CancellationToken ct)
        {
            var order = await orderQuery.GetByIdAsync(
                    request.OrderId,
                    ct,
                    q => q
                        .Include(o => o.Address)
                        .Include(o => o.OrderItems)
                        .Include(o => o.Customer)
                        .ThenInclude(c => c.Contact));

            if (order == null)
                throw new Exception("Замовлення не знайдено");

            if (order.Address == null)
                throw new Exception("У замовленні відсутня адреса доставки");

            var companyId = await currentUserService.GetCompanyIdAsync(ct) ?? throw new Exception("Користувач не авторизований");
            var isBranchDelivery = order.Address.DeliveryPointType == DeliveryPointType.Branch;

            if (order.Address.NovaPoshtaCityRef is null || order.Address.NovaPoshtaWarehouseRef is null)
            {
                var city = await context.NovaPoshtaCities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Description.ToLower() == order.Address.City.ToLower(), ct);

                if (city == null)
                    throw new Exception($"Не вдалося автоматично знайти місто '{order.Address.City}' у довіднику Нової Пошти.");

                var deptNumber = order.Address.PostDepartmentNumber;
                var deptNumberText = deptNumber.ToString();

                NovaPoshtaWarehouse? warehouse = null;

                warehouse = await context.NovaPoshtaWarehouses
                    .AsNoTracking()
                    .Where(x => x.CityRef == city.Ref && x.Number == deptNumberText)
                    .FirstOrDefaultAsync(ct);

                if (warehouse == null && isBranchDelivery)
                {
                    var exactBranchDesc = $"Відділення №{deptNumberText}";
                    var exactQuerySpace = $"№{deptNumberText} ";
                    var exactQueryComma = $"№{deptNumberText},";

                    warehouse = await context.NovaPoshtaWarehouses
                        .AsNoTracking()
                        .Where(x => x.CityRef == city.Ref)
                        .Where(x => !x.Description.ToLower().Contains("поштомат"))
                        .FirstOrDefaultAsync(x =>
                            x.Description.Contains(exactBranchDesc) ||
                            x.Description.Contains(exactQuerySpace) ||
                            x.Description.Contains(exactQueryComma), ct);
                }
                else if (warehouse == null)
                {
                    warehouse = await context.NovaPoshtaWarehouses
                        .AsNoTracking()
                        .Where(x => x.CityRef == city.Ref)
                        .Where(x => x.Description.ToLower().Contains("поштомат"))
                        .FirstOrDefaultAsync(x => x.Number == deptNumberText, ct);
                }
                if (warehouse == null)
                {
                    var npWarehouses = await npClient.ExecuteAsync<GetNPWarehousesProperties, NovaPoshtaWarehouseDto>(
                        companyId,
                        "Address",
                        "getWarehouses",
                        new GetNPWarehousesProperties(city.Ref),
                        ct);

                    NovaPoshtaWarehouseDto? fromApi = null;

                    if (isBranchDelivery)
                    {
                        var exactBranchDesc = $"Відділення №{deptNumberText}";
                        var exactQuerySpace = $"№{deptNumberText} ";
                        var exactQueryComma = $"№{deptNumberText},";

                        fromApi = npWarehouses
                            .Where(x => !x.Description.ToLower().Contains("поштомат"))
                            .FirstOrDefault(x =>
                                x.Number == deptNumberText ||
                                x.Description.Contains(exactBranchDesc) ||
                                x.Description.Contains(exactQuerySpace) ||
                                x.Description.Contains(exactQueryComma));
                    }
                    else
                    {
                        fromApi = npWarehouses
                            .Where(x => x.Description.ToLower().Contains("поштомат"))
                            .FirstOrDefault(x => x.Number == deptNumberText);
                    }

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
                    throw new Exception($"Не вдалося автоматично знайти {(isBranchDelivery ? "відділення" : "поштомат")} №{deptNumber} у місті '{order.Address.City}' в довіднику НП.");

                order.Address.SetNovaPoshtaRefs(city.Ref, warehouse.Ref);
            }

            if (order.Address.NovaPoshtaCityRef is null || order.Address.NovaPoshtaWarehouseRef is null)
                throw new Exception("Для адреси замовлення не заповнено Nova Poshta city/warehouse refs");

            if (order.Customer is null || order.Customer.Contact is null)
                throw new Exception("У замовленні відсутні дані отримувача (Customer/Contact)");

            var integration = await context.UserIntegrations
                .Include(x => x.LogisticProfile)
                .ThenInclude(lp => lp.SenderAddresses)
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Provider == "novaposhta", ct);

            if (integration == null)
                throw new Exception("Інтеграція з Новою Поштою не налаштована");

            var senderProfile = integration.LogisticProfile
                ?? throw new Exception("Профіль відправника Нової Пошти не заповнено");

            var senderAddress = senderProfile.SenderAddresses.FirstOrDefault()
                ?? throw new Exception("Не знайдено жодної адреси відправника у профілі логістики");

            var counterpartyProperties = new
            {
                FirstName = order.Customer.Name,
                MiddleName = "",
                LastName = order.Customer.Surname,
                Phone = order.Customer.Contact.PhoneNumber,
                Email = order.Customer.Email,
                CounterpartyType = "PrivatePerson",
                CounterpartyProperty = "Recipient"
            };

            var counterpartyResponse = await npClient.ExecuteAsync<object, NpCreateCounterpartyResult>(
                companyId,
                "Counterparty",
                "save",
                counterpartyProperties,
                ct);

            var cpData = counterpartyResponse.FirstOrDefault();

            if (cpData == null)
                throw new Exception("Не вдалося створити контрагента в Новій Пошті. Відповідь порожня.");

            string recipientRef = cpData.Ref;

            string contactRecipientRef = cpData.ContactPerson?.Data?.FirstOrDefault()?.Ref
                ?? throw new Exception("Нова Пошта не повернула контактну особу для створеного контрагента.");

            string npServiceType = order.ServiceType switch
            {
                ServiceType.WarehouseWarehouse => "WarehouseWarehouse",
                ServiceType.DoorsDoors => "DoorsDoors",
                ServiceType.DoorsWarehouse => "DoorsWarehouse",
                ServiceType.WarehouseDoors => "WarehouseDoors",
                _ => "WarehouseWarehouse"
            };

            string npPayerType = order.PaymentMethods switch
            {
                PaymentMethods.AfterPayment => "Recipient",
                PaymentMethods.FullPayment => "Recipient",
                _ => "Sender"
            };
            string npCargoType = order.CargoType switch
            {
                CargoType.Documents => "Documents",
                CargoType.Cargo => "Cargo",
                CargoType.Parcel => "Parcel",
                _ => "Parcel"
            };

            var packageWeight = NormalizePackageWeight(order.TotalWeight);
            var packageVolume = CalculatePackageVolume(packageWeight);

            var npRequest = new NpCreateInternetDocumentProperties
            {
               // SenderWarehouseIndex = senderAddress.WarehouseIndex,
                //RecipientWarehouseIndex = isBranchDelivery ? "101/102" : "11/3002",
                PayerType = npPayerType,
                PaymentMethod = "Cash",
                DateTime = DateTime.Now.ToString("dd.MM.yyyy"),
                CargoType = npCargoType,
                Sender = senderProfile.SenderRef,
                CitySender = senderAddress.CityRef,
                SenderAddress = senderAddress.AddressRef,
                ContactSender = senderProfile.ContactSenderRef,
                SendersPhone = senderProfile.SendersPhone,
                Recipient = recipientRef,
                CityRecipient = order.Address.NovaPoshtaCityRef,
                RecipientAddress = order.Address.NovaPoshtaWarehouseRef,
                ContactRecipient = contactRecipientRef,
                RecipientsPhone = order.Customer.Contact.PhoneNumber,
                Weight = packageWeight,
                Cost = order.TotalCost <= 0 ? 1m : order.TotalCost,
                Description = string.IsNullOrWhiteSpace(order.Description) ? $"Замовлення {order.UniqCode}" : order.Description,
                SeatsAmount = "1",
                ServiceType = npServiceType
            };

            if (isBranchDelivery)
            {
                npRequest.VolumeGeneral = packageVolume;
            }
            else
            {
                npRequest.OptionsSeat =
                [
                    new NpOptionsSeatItem
                    {
                        VolumetricVolume = packageVolume.ToString("0.###", CultureInfo.InvariantCulture),
                        Weight = packageWeight.ToString("0.###", CultureInfo.InvariantCulture)
                    }
                ];
            }

            var response = await npClient.ExecuteAsync<NpCreateInternetDocumentProperties, NpCreateInternetDocumentResult>(
                companyId,
                "InternetDocument",
                "save",
                npRequest,
                ct);

            var createdDocument = response.FirstOrDefault()
                ?? throw new Exception("Нова Пошта не повернула дані створеної ТТН");

            order.SetTrackingNumber(createdDocument.IntDocNumber);
            await context.SaveChangesAsync(ct);

            return createdDocument.IntDocNumber;
        }

        private static decimal NormalizePackageWeight(decimal totalWeight)
        {
            return totalWeight > 0 ? Math.Max(totalWeight, 0.1m) : 0.3m;
        }

        private static decimal CalculatePackageVolume(decimal weight)
        {
            var volume = weight / 250m;
            return Math.Clamp(volume, 0.002m, 0.12m);
        }
    }
}





