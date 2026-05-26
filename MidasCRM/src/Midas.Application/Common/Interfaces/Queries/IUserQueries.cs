using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IUserQueries
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
