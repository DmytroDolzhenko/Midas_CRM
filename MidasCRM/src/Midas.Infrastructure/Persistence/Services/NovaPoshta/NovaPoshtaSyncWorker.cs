using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NovaPoshtaSyncWorker> _logger;
        public NovaPoshtaSyncWorker(IServiceProvider serviceProvider, ILogger<NovaPoshtaSyncWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Nova Poshta Sync Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.UtcNow.AddHours(2); // Київський час
                    var nextRun = now.Date.AddDays(3).AddHours(3); // 03:00 через три дні
                    var delay = nextRun - now;

                    _logger.LogInformation($"Next Nova Poshta sync scheduled at: {nextRun}");
                    await Task.Delay(delay, stoppingToken);
                    //await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var syncService = scope.ServiceProvider.GetRequiredService<NovaPoshtaSyncService>();

                        var systemAdminUserId = Guid.Parse("44896f13-e34d-47ca-af83-8bef28b2e984");

                        _logger.LogInformation("Starting background synchronization of Nova Poshta directories...");
                        await syncService.SyncAllDataAsync(systemAdminUserId, stoppingToken);
                        _logger.LogInformation("Nova Poshta directories successfully updated.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during Nova Poshta background synchronization.");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
