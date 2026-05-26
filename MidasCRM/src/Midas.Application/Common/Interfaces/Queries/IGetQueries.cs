using System.Linq;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IGetQueries<T, TKey> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync(
            CancellationToken cancellationToken,
            Func<IQueryable<T>, IQueryable<T>>? queryShaper = null);

        Task<T?> GetByIdAsync(
            TKey id,
            CancellationToken cancellationToken,
            Func<IQueryable<T>, IQueryable<T>>? queryShaper = null);
    }
}
