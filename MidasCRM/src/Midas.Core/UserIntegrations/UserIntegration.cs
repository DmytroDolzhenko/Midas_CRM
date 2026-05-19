using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.UserIntegrations
{
    public class UserIntegration : IEntity<int>
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; }
        public string EncryptedAccessToken { get; set; }
        public string? EncryptedRefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
