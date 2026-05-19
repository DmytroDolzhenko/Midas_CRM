using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.UserIntegrations;

namespace Midas.Application.Entities.ConnectionServices.Commands
{
    public record ConnectExternalServiceCommand(string Provider, string Code, Guid? UserId) : IRequest<bool>;
    public record SaveStaticTokenCommand(string Provider, string Token) : IRequest<bool>;

    public class ConnectExternalServiceHandler : IRequestHandler<ConnectExternalServiceCommand, bool>
    {
        private readonly IEnumerable<IIntegrationProvider> _providers;
        private readonly IEncryptionService _encryption;
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public ConnectExternalServiceHandler(
            IEnumerable<IIntegrationProvider> providers,
            IEncryptionService encryption,
            IApplicationDbContext context,
            ICurrentUserService currentUser)
        {
            _providers = providers;
            _encryption = encryption;
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(ConnectExternalServiceCommand request, CancellationToken ct)
        {
            var userId = request.UserId ?? _currentUser.UserId;
            if (userId is null)
            {
                return false;
            }

            var provider = _providers.FirstOrDefault(p => p.ProviderName == request.Provider);
            if (provider == null)
            {
                return false;
            }

            var tokens = await provider.ExchangeCodeAsync(request.Code);
            var integration = _context.UserIntegrations
                .FirstOrDefault(x => x.UserId == userId.Value && x.Provider == request.Provider);

            if (integration is null)
            {
                integration = new UserIntegration
                {
                    UserId = userId.Value,
                    Provider = request.Provider,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserIntegrations.Add(integration);
            }

            integration.EncryptedAccessToken = _encryption.Encrypt(tokens.AccessToken);
            integration.EncryptedRefreshToken = tokens.RefreshToken != null
                ? _encryption.Encrypt(tokens.RefreshToken)
                : null;
            integration.ExpiresAt = tokens.ExpiresIn > 0
                ? DateTime.UtcNow.AddSeconds(tokens.ExpiresIn)
                : null;
            integration.IsActive = true;

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }

    public class SaveStaticTokenHandler : IRequestHandler<SaveStaticTokenCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEncryptionService _encryption;
        private readonly ICurrentUserService _currentUser;

        public SaveStaticTokenHandler(
            IApplicationDbContext context,
            IEncryptionService encryption,
            ICurrentUserService currentUser)
        {
            _context = context;
            _encryption = encryption;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(SaveStaticTokenCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId is null || string.IsNullOrWhiteSpace(request.Token))
            {
                return false;
            }

            var userId = _currentUser.UserId.Value;
            var integration = _context.UserIntegrations
                .FirstOrDefault(x => x.UserId == userId && x.Provider == request.Provider);

            if (integration is null)
            {
                integration = new UserIntegration
                {
                    UserId = userId,
                    Provider = request.Provider,
                    CreatedAt = DateTime.UtcNow
                };
                _context.UserIntegrations.Add(integration);
            }

            integration.EncryptedAccessToken = _encryption.Encrypt(request.Token);
            integration.EncryptedRefreshToken = null;
            integration.ExpiresAt = null;
            integration.IsActive = true;

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
