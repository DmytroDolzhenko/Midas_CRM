using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Repositories
{
    public interface IEntityRepository<T> where T : class
    {
        public Task AddAsync(T entity, CancellationToken cancellationToken);
        public Task UpdateAsync(T entity, CancellationToken cancellationToken);
        public Task DeleteAsync(T entity, CancellationToken cancellationToken);
    }
}
