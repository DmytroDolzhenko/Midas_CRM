using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaOrderTrackingWorker(
        IServiceProvider serviceProvider,
        ILogger<NovaPoshtaOrderTrackingWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            /*            logger.LogInformation("Nova Poshta order tracking worker started.");

                        // Перший запуск відбудеться через 20 секунд після старту додатку
                        await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);

                        while (!stoppingToken.IsCancellationRequested)
                        {
                            try
                            {
                                using var scope = serviceProvider.CreateScope();
                                var trackingService = scope.ServiceProvider.GetRequiredService<OrderTrackingService>();

                                logger.LogInformation("Starting Nova Poshta order status sync...");
                                await trackingService.SyncStatusesAsync(stoppingToken);
                                logger.LogInformation("Nova Poshta order statuses sync completed.");

                                await Task.Delay(TimeSpan.FromSeconds(20), stoppingToken);
                            }
                            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                            {
                                break;
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex, "Error occurred during Nova Poshta order statuses sync.");
                                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                            }
                        }*/
            logger.LogInformation("Nova Poshta order tracking worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var kyivTimeZone = ResolveKyivTimeZone();
                    var nowUtc = DateTime.UtcNow;
                    var nowKyiv = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, kyivTimeZone);
                    var nextRunKyiv = nowKyiv.Date.AddHours(4);

                    if (nowKyiv >= nextRunKyiv)
                    {
                        nextRunKyiv = nextRunKyiv.AddDays(1);
                    }

                    var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRunKyiv, kyivTimeZone);
                    var delay = nextRunUtc - nowUtc;

                    logger.LogInformation("Next Nova Poshta order status sync scheduled at {NextRunKyiv} (Kyiv time).", nextRunKyiv);
                    await Task.Delay(delay, stoppingToken);

                    using var scope = serviceProvider.CreateScope();
                    var trackingService = scope.ServiceProvider.GetRequiredService<OrderTrackingService>();
                    await trackingService.SyncStatusesAsync(stoppingToken);

                    logger.LogInformation("Nova Poshta order statuses sync completed.");
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during Nova Poshta order statuses sync.");
                    await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
                }
            }
        }

        private static TimeZoneInfo ResolveKyivTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Europe/Kyiv");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("FLE Standard Time");
            }
        }
    }
}
