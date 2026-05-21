using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Midas.Application.Common.Interfaces;
using Midas.Application.DTO.NovaPoshta.Requests;
using Midas.Application.DTOs.NovaPoshta;
using Midas.Core.NovaPoshta;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaSyncService
    {
        private readonly IApplicationDbContext _context;
        private readonly INovaPoshtaClient _npClient;
        private readonly ILogger<NovaPoshtaSyncService> _logger;

        public NovaPoshtaSyncService(
            IApplicationDbContext context,
            INovaPoshtaClient npClient,
            ILogger<NovaPoshtaSyncService> logger)
        {
            _context = context;
            _npClient = npClient;
            _logger = logger;
        }

        public async Task SyncAllDataAsync(Guid systemCompanyId, CancellationToken ct)
        {
            _logger.LogInformation("Nova Poshta sync started for company {CompanyId}", systemCompanyId);

            var npCities = await _npClient.ExecuteAsync<GetAddressCitiesRequest, NpCityItem>(
                systemCompanyId, "Address", "getCities", new GetAddressCitiesRequest(), ct);
            _logger.LogInformation("Received {Count} cities from Nova Poshta API", npCities?.Count ?? 0);

            if (npCities != null && npCities.Any())
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"nova_poshta_cities\"", ct);
                _logger.LogInformation("Truncated nova_poshta_cities");

                var cityEntities = npCities.Select(c => NovaPoshtaCity.Create(
                    c.Ref, c.Description, c.SettlementTypeDescription, c.AreaDescription)).ToList();

                foreach (var batch in cityEntities.Chunk(2000))
                {
                    await _context.NovaPoshtaCities.AddRangeAsync(batch, ct);
                    await _context.SaveChangesAsync(ct);
                }
                _logger.LogInformation("Inserted {Count} cities into DB", cityEntities.Count);
            }

            var allWarehouses = new List<NpWarehouseItem>();
            var page = 1;

            while (true)
            {
                var pageResult = await _npClient.ExecuteAsync<GetWarehousesRequest, NpWarehouseItem>(
                    systemCompanyId,
                    "Address",
                    "getWarehouses",
                    new GetWarehousesRequest(Page: page.ToString(), Limit: "500"),
                    ct);

                if (pageResult == null || pageResult.Count == 0)
                {
                    break;
                }

                allWarehouses.AddRange(pageResult);
                _logger.LogInformation("Loaded warehouses page {Page}, count {Count}", page, pageResult.Count);
                page++;
            }

            _logger.LogInformation("Received total {Count} warehouses from Nova Poshta API", allWarehouses.Count);

            if (allWarehouses.Any())
            {
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"nova_poshta_warehouses\"", ct);
                _logger.LogInformation("Truncated nova_poshta_warehouses");
                var warehouseEntities = allWarehouses.Select(w => NovaPoshtaWarehouse.Create(
                    w.Ref, w.CityRef, w.Description, w.Number, w.WarehouseIndex, w.TypeOfWarehouse)).ToList();

                foreach (var batch in warehouseEntities.Chunk(2000))
                {
                    await _context.NovaPoshtaWarehouses.AddRangeAsync(batch, ct);
                    await _context.SaveChangesAsync(ct);
                }
                _logger.LogInformation("Inserted {Count} warehouses into DB", warehouseEntities.Count);
            }

            _logger.LogInformation("Nova Poshta sync finished for company {CompanyId}", systemCompanyId);
        }
    }
}
