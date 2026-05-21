using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Task<Guid?> GetCompanyIdAsync(CancellationToken cancellationToken);
    }
}
