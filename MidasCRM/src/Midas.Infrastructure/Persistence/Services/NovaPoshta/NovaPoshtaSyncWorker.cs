using System;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaSyncWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NovaPoshtaSyncWorker> _logger;
        private readonly IConfiguration _configuration;

        public NovaPoshtaSyncWorker(IServiceProvider serviceProvider, ILogger<NovaPoshtaSyncWorker> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
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

                    using var scope = _serviceProvider.CreateScope();
                    var syncService = scope.ServiceProvider.GetRequiredService<NovaPoshtaSyncService>();

                    Guid? systemCompanyId = null;

                    var systemCompanyIdString = _configuration["NovaPoshtaSettings:SystemCompanyId"];
                    if (!string.IsNullOrWhiteSpace(systemCompanyIdString)
                        && Guid.TryParse(systemCompanyIdString, out var parsedCompanyId))
                    {
                        systemCompanyId = parsedCompanyId;
                    }
                    else
                    {
                        var adminUserIdString = _configuration["NovaPoshtaSettings:SystemAdminUserId"];
                        if (!string.IsNullOrWhiteSpace(adminUserIdString)
                            && Guid.TryParse(adminUserIdString, out var adminUserId))
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                            systemCompanyId = await dbContext.CompanyMembers
                                .Where(x => x.UserId == adminUserId)
                                .Select(x => (Guid?)x.CompanyId)
                                .FirstOrDefaultAsync(stoppingToken);
                        }
                    }

                    if (systemCompanyId is null)
                    {
                        _logger.LogError("System company ID is missing/invalid and cannot be resolved by SystemAdminUserId.");
                        continue;
                    }

                    _logger.LogInformation("Starting background synchronization of Nova Poshta directories...");
                    await syncService.SyncAllDataAsync(systemCompanyId.Value, stoppingToken);
                    _logger.LogInformation("Nova Poshta directories successfully updated.");
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
