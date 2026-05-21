using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces
{
    public interface INovaPoshtaClient
    {
        Task<List<TResponse>> ExecuteAsync<TRequest, TResponse>(
            Guid companyId,
            string modelName,
            string calledMethod,
            TRequest properties,
            CancellationToken ct);
    }
}
