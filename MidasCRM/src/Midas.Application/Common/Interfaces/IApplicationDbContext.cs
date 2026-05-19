using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Midas.Core.UserIntegrations;

namespace Midas.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DatabaseFacade Database { get; }
        DbSet<UserIntegration> UserIntegrations { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
