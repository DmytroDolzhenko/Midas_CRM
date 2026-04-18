using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IGetQueries<T, TKey> where T : class
    {
        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);
        public Task<T?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
    }
}
    