using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.DTO.NovaPoshta.Requests
{
    // Для міст
    public record GetAddressCitiesRequest(string Property = "");
    public record NpCityItem(string Ref, string Description, string SettlementTypeDescription, string AreaDescription);

    // Для складів
    public record GetWarehousesRequest(string CityRef = "");
    public record NpWarehouseItem(string Ref, string CityRef, string Description, string Number, string WarehouseIndex, string TypeOfWarehouse);
}
