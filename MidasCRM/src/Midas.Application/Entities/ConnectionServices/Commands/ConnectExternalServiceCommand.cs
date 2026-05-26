using MediatR;
using Microsoft.EntityFrameworkCore;
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
            if (userId is null) return false;
            var companyId = await _context.CompanyMembers
                .Where(x => x.UserId == userId.Value)
                .Select(x => (Guid?)x.CompanyId)
                .FirstOrDefaultAsync(ct);
            if (companyId is null) return false;

            var provider = _providers.FirstOrDefault(p => p.ProviderName == request.Provider);
            if (provider == null) return false;

            var tokens = await provider.ExchangeCodeAsync(request.Code);

            var integration = _context.UserIntegrations
                .FirstOrDefault(x => x.CompanyId == companyId.Value && x.Provider == request.Provider);

            if (integration is null)
            {
                integration = UserIntegration.Create(companyId.Value, userId.Value, request.Provider);
                _context.UserIntegrations.Add(integration);
            }

            var encryptedAccess = _encryption.Encrypt(tokens.AccessToken);
            var encryptedRefresh = tokens.RefreshToken != null ? _encryption.Encrypt(tokens.RefreshToken) : null;

            integration.UpdateTokens(encryptedAccess, encryptedRefresh, tokens.ExpiresIn);

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
            if (_currentUser.UserId is null || string.IsNullOrWhiteSpace(request.Token)) return false;

            var userId = _currentUser.UserId.Value;
            var companyId = await _context.CompanyMembers
                .Where(x => x.UserId == userId)
                .Select(x => (Guid?)x.CompanyId)
                .FirstOrDefaultAsync(ct);
            if (companyId is null) return false;

            // Завантажуємо разом з профілем, щоб перевірити наявність
            var integration = await _context.UserIntegrations
                .Include(x => x.LogisticProfile)
                .FirstOrDefaultAsync(x => x.CompanyId == companyId.Value && x.Provider == request.Provider, ct);

            if (integration is null)
            {
                integration = UserIntegration.Create(companyId.Value, userId, request.Provider);
                _context.UserIntegrations.Add(integration);

                // Зберігаємо проміжний стан, щоб згенерувався числовий ID для integration.Id,
                // який потрібен для зовнішнього ключа UserLogisticProfile
                await _context.SaveChangesAsync(ct);
            }

            if (integration.LogisticProfile == null)
            {
                // Використовуємо новий фабричний метод, щоб не ламати інкапсуляцію DDD
                var emptyProfile = UserLogisticProfile.CreateEmpty(integration.Id);
                _context.UserLogisticProfiles.Add(emptyProfile);
            }

            var encryptedToken = _encryption.Encrypt(request.Token);
            integration.UpdateStaticToken(encryptedToken);

            await _context.SaveChangesAsync(ct);
            return true;
        }
    }
}
