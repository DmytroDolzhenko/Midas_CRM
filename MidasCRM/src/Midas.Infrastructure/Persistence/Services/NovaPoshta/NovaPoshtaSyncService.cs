using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public NovaPoshtaSyncService(IApplicationDbContext context, INovaPoshtaClient npClient)
        {
            _context = context;
            _npClient = npClient;
        }

        public async Task SyncAllDataAsync(Guid systemUserId, CancellationToken ct)
        {
            // 1. СИНХРОНІЗАЦІЯ МІСТ
            var npCities = await _npClient.ExecuteAsync<GetAddressCitiesRequest, NpCityItem>(
                systemUserId, "Address", "getCities", new GetAddressCitiesRequest(), ct);

            if (npCities != null && npCities.Any())
            {
                // Очищаємо стару таблицю міст (простий варіант без BulkExtensions)
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"nova_poshta_cities\"", ct);

                // Заливати будемо пачками по 2000 штук, щоб EF Core не з'їв усю пам'ять
                var cityEntities = npCities.Select(c => NovaPoshtaCity.Create(
                    c.Ref, c.Description, c.SettlementTypeDescription, c.AreaDescription)).ToList();

                foreach (var batch in cityEntities.Chunk(2000))
                {
                    await _context.NovaPoshtaCities.AddRangeAsync(batch, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }

            // 2. СИНХРОНІЗАЦІЯ СКЛАДІВ
            var npWarehouses = await _npClient.ExecuteAsync<GetWarehousesRequest, NpWarehouseItem>(
                systemUserId, "Address", "getWarehouses", new GetWarehousesRequest(), ct);

            if (npWarehouses != null && npWarehouses.Any())
            {
                // Очищаємо стару таблицю складів
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE \"nova_poshta_warehouses\"", ct);
                var warehouseEntities = npWarehouses.Select(w => NovaPoshtaWarehouse.Create(
                    w.Ref, w.CityRef, w.Description, w.Number, w.WarehouseIndex, w.TypeOfWarehouse)).ToList();

                foreach (var batch in warehouseEntities.Chunk(2000))
                {
                    await _context.NovaPoshtaWarehouses.AddRangeAsync(batch, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }
        }
    }
}
