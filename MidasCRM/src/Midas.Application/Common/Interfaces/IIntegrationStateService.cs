using System;

namespace Midas.Application.Common.Interfaces
{
    public interface IIntegrationStateService
    {
        string CreateState(Guid userId, string provider);
        bool TryValidateState(string state, string provider, out Guid userId);
    }
}
