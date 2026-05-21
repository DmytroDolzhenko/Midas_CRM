using Midas.Application.Common.Interfaces;
using Midas.Application.DTO.NovaPoshta.Responses;
using Midas.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class OrderTrackingService(
        IApplicationDbContext context,
        INovaPoshtaClient npClient)
    {
        public async Task SyncStatusesAsync(CancellationToken ct)
        {
            var orders = await context.Orders
                .Include(o => o.Payments)
                .Where(o =>
                    !string.IsNullOrWhiteSpace(o.TrackingNumber) &&
                    o.Status != OrderStatus.Cancelled &&
                    !o.IsDeleted)
                .ToListAsync(ct);

            if (!orders.Any()) return;

            var groupedOrders = orders.GroupBy(o => o.CompanyId);

            foreach (var group in groupedOrders)
            {
                var userId = group.Key;
                var ttnList = group.Select(o => o.TrackingNumber!).ToList();

                var trackingData = await npClient.ExecuteAsync<object, NpTrackingStatusResponse>(
                    userId,
                    "TrackingDocument",
                    "getStatusDocuments",
                    new { Documents = ttnList.Select(ttn => new { DocumentNumber = ttn }) },
                    ct);

                foreach (var item in trackingData)
                {
                    var order = group.FirstOrDefault(o => o.TrackingNumber == item.Number);
                    if (order is null) continue;

                    var mappedStatus = MapNpdStatus(item.StatusCode);
                    if (mappedStatus != order.Status)
                    {
                        order.UpdateStatus(mappedStatus);

                        if (mappedStatus == OrderStatus.Delivered || mappedStatus == OrderStatus.Received)
                        {
                            order.CompleteAllPayments();
                        }
                    }
                }
            }

            await context.SaveChangesAsync(ct);
        }

        private static OrderStatus MapNpdStatus(string statusCode) => statusCode switch
        {

            "2" => OrderStatus.Deleted,
            "3" => OrderStatus.Delivered,
            "5" => OrderStatus.Received,
            _ => OrderStatus.Processing
        };
    }
}

