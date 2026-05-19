using Microsoft.Extensions.Configuration;
using Midas.Application.Common.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Midas.Infrastructure.Persistence.Services
{
    public class HmacIntegrationStateService(IConfiguration configuration) : IIntegrationStateService
    {
        private readonly byte[] _key = Encoding.UTF8.GetBytes(
            configuration["Integration:StateSigningKey"]
            ?? configuration["JwtSettings:Key"]
            ?? throw new InvalidOperationException("Integration:StateSigningKey or JwtSettings:Key is required."));

        public string CreateState(Guid userId, string provider)
        {
            var payload = JsonSerializer.Serialize(new IntegrationStatePayload
            {
                UserId = userId,
                Provider = provider,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(15)
            });

            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var signatureBytes = Sign(payloadBytes);

            return $"{Base64UrlEncode(payloadBytes)}.{Base64UrlEncode(signatureBytes)}";
        }

        public bool TryValidateState(string state, string provider, out Guid userId)
        {
            userId = Guid.Empty;

            var parts = state.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            try
            {
                var payloadBytes = Base64UrlDecode(parts[0]);
                var signatureBytes = Base64UrlDecode(parts[1]);

                var expected = Sign(payloadBytes);
                if (!CryptographicOperations.FixedTimeEquals(signatureBytes, expected))
                {
                    return false;
                }

                var payload = JsonSerializer.Deserialize<IntegrationStatePayload>(payloadBytes);
                if (payload is null)
                {
                    return false;
                }

                if (!string.Equals(payload.Provider, provider, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (payload.ExpiresAtUtc < DateTime.UtcNow)
                {
                    return false;
                }

                userId = payload.UserId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private byte[] Sign(byte[] payload)
        {
            using var hmac = new HMACSHA256(_key);
            return hmac.ComputeHash(payload);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string value)
        {
            var padded = value.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2:
                    padded += "==";
                    break;
                case 3:
                    padded += "=";
                    break;
            }

            return Convert.FromBase64String(padded);
        }

        private sealed class IntegrationStatePayload
        {
            public Guid UserId { get; set; }
            public string Provider { get; set; } = string.Empty;
            public DateTime ExpiresAtUtc { get; set; }
        }
    }
}
