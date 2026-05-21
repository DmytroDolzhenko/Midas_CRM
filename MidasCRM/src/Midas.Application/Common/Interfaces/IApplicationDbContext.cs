using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using Midas.Core.FinancialOperations;
using Midas.Core.NovaPoshta;
using Midas.Core.Orders;
using Midas.Core.ProductCategories;
using Midas.Core.UserProductCategories;
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
        DbSet<FinancialOperation> FinancialOperations { get; }
        DbSet<ProductCategory> ProductCategories { get; }
        DbSet<UserProductCategory> UserProductCategories { get; }
        DbSet<NovaPoshtaCity> NovaPoshtaCities { get; }
        DbSet<NovaPoshtaWarehouse> NovaPoshtaWarehouses { get; }
        DbSet<Company> Companies { get; }
        DbSet<CompanyMember> CompanyMembers { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
