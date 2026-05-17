using Midas.Application.Common.Interfaces.Repositories;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Midas.Infrastructure.Persistence.Services
{
    public sealed class OAuthProviderSettings
    {
        public string Name { get; set; } = string.Empty;
        public string AuthorizeUrl { get; set; } = string.Empty;
        public string TokenUrl { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool UseBasicAuthForTokenRequest { get; set; }
        public bool Enabled { get; set; } = true;
    }

    public class GenericOAuthIntegrationProvider : IIntegrationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly OAuthProviderSettings _settings;

        public GenericOAuthIntegrationProvider(HttpClient httpClient, OAuthProviderSettings settings)
        {
            _httpClient = httpClient;
            _settings = settings;
        }

        public string ProviderName => _settings.Name;

        public string BuildAuthorizeUrl(string state)
        {
            var query = new Dictionary<string, string?>
            {
                ["response_type"] = "code",
                ["client_id"] = _settings.ClientId,
                ["redirect_uri"] = _settings.RedirectUri,
                ["scope"] = _settings.Scope,
                ["state"] = state
            };

            var queryString = string.Join("&", query
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}"));

            return $"{_settings.AuthorizeUrl}?{queryString}";
        }

        public async Task<TokenResponse> ExchangeCodeAsync(string code)
        {
            var body = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = _settings.RedirectUri
            };

            AddClientCredentials(body);
            var request = BuildTokenRequest(body);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return ParseToken(json);
        }

        public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken
            };

            AddClientCredentials(body);
            var request = BuildTokenRequest(body);

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return ParseToken(json);
        }

        private HttpRequestMessage BuildTokenRequest(Dictionary<string, string> body)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _settings.TokenUrl)
            {
                Content = new FormUrlEncodedContent(body)
            };

            if (_settings.UseBasicAuthForTokenRequest)
            {
                var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
            }

            return request;
        }

        private void AddClientCredentials(Dictionary<string, string> body)
        {
            if (_settings.UseBasicAuthForTokenRequest)
            {
                return;
            }

            body["client_id"] = _settings.ClientId;
            body["client_secret"] = _settings.ClientSecret;
        }

        private static TokenResponse ParseToken(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var accessToken = root.TryGetProperty("access_token", out var accessTokenElement)
                ? accessTokenElement.GetString()
                : null;
            var refreshToken = root.TryGetProperty("refresh_token", out var refreshTokenElement)
                ? refreshTokenElement.GetString()
                : null;
            var tokenType = root.TryGetProperty("token_type", out var tokenTypeElement)
                ? tokenTypeElement.GetString()
                : "Bearer";
            var expiresIn = root.TryGetProperty("expires_in", out var expiresInElement)
                && expiresInElement.ValueKind == JsonValueKind.Number
                ? expiresInElement.GetInt32()
                : 0;

            if (string.IsNullOrWhiteSpace(accessToken)) 
            {
                throw new InvalidOperationException("Provider token response does not contain access_token.");
            }

            return new TokenResponse(accessToken, refreshToken, expiresIn, tokenType);
        }
    }
}
