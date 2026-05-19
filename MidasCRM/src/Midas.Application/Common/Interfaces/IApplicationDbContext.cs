using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Midas.Core.NovaPoshta;
using Midas.Core.Orders;
using Midas.Core.UserIntegrations;
using System.Threading;
using System.Threading.Tasks;

namespace Midas.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DatabaseFacade Database { get; }
        DbSet<UserIntegration> UserIntegrations { get; }
        DbSet<UserLogisticProfile> UserLogisticProfiles { get; }
        DbSet<Order> Orders { get; }
        DbSet<NovaPoshtaCity> NovaPoshtaCities { get; }
        DbSet<NovaPoshtaWarehouse> NovaPoshtaWarehouses { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
