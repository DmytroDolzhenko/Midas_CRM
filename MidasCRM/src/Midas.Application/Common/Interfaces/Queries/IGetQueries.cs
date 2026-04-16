using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IGetQueries<T> where T : class
    {
        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken);
        public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
    