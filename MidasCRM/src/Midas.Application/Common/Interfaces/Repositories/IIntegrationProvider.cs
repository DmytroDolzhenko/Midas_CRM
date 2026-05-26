using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Repositories
{
    public record TokenResponse(
     string AccessToken,
     string? RefreshToken,
     int ExpiresIn,
     string? TokenType = "Bearer"
    );
    public interface IIntegrationProvider
    {
        string ProviderName { get; }
        string BuildAuthorizeUrl(string state);
        Task<TokenResponse> ExchangeCodeAsync(string code);
        Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    }
}
