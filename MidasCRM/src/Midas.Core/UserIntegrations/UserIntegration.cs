using Midas.Core.Companies;
using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.UserIntegrations
{
    public class UserIntegration : IEntity<int>
    {
        public int Id { get; private set; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;
        //public Guid UserId { get; private set; }
        public string Provider { get; private set; } = null!;
        public string? EncryptedAccessToken { get; private set; } = null!;
        public string? EncryptedRefreshToken { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public UserLogisticProfile? LogisticProfile { get; private set; }
        public Guid CreatedByUserId { get; private set; }
        public User CreatedByUser { get; private set; } = null!;

        private UserIntegration() { }

        public void SetLogisticProfile(UserLogisticProfile profile)
        {
            LogisticProfile = profile;
        }
        public static UserIntegration Create(Guid commanyId, Guid createdByUserId, string provider)
        {
            return new UserIntegration
            {
                CompanyId = commanyId,
                CreatedByUserId = createdByUserId,
                Provider = provider,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
        public void UpdateTokens(string encryptedAccessToken, string? encryptedRefreshToken, int expiresInSeconds)
        {
            EncryptedAccessToken = encryptedAccessToken;
            EncryptedRefreshToken = encryptedRefreshToken;
            ExpiresAt = expiresInSeconds > 0 ? DateTime.UtcNow.AddSeconds(expiresInSeconds) : null;
            IsActive = true;
        }
        public void UpdateStaticToken(string encryptedToken)
        {
            EncryptedAccessToken = encryptedToken;
            EncryptedRefreshToken = null;
            ExpiresAt = null;
            IsActive = true;
        }
    }
}
