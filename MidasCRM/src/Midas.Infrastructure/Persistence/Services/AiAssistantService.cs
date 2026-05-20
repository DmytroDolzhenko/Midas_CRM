using Microsoft.Extensions.Options;
using Midas.Application.Common.Interfaces.Repositories;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Midas.Infrastructure.Persistence.Services
{
    public sealed class AiAssistantSettings
    {
        public string BaseUrl { get; init; } = string.Empty;
        public string ModelName { get; init; } = string.Empty;
    }

    public sealed class AiAssistantService : IAiAssistantService
    {
        private readonly HttpClient _httpClient;
        private readonly AiAssistantSettings _settings;

        public AiAssistantService(HttpClient httpClient, IOptions<AiAssistantSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> GetRecommendationAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
            {
                throw new InvalidOperationException("AiSettings:BaseUrl is not configured.");
            }

            if (string.IsNullOrWhiteSpace(_settings.ModelName))
            {
                throw new InvalidOperationException("AiSettings:ModelName is not configured.");
            }

            var request = new ChatCompletionRequest(
                _settings.ModelName,
                [
                    new ChatMessage("system", systemPrompt),
                    new ChatMessage("user", userPrompt)
                ],
                false);

            using var response = await _httpClient.PostAsJsonAsync(_settings.BaseUrl, request, cancellationToken);
            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = result?.Error?.Message ?? response.ReasonPhrase ?? "AI service request failed.";
                throw new InvalidOperationException(message);
            }

            return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim()
                ?? "AI сервіс не повернув рекомендації.";
        }

        private sealed record ChatCompletionRequest(
            [property: JsonPropertyName("model")] string Model,
            [property: JsonPropertyName("messages")] IReadOnlyCollection<ChatMessage> Messages,
            [property: JsonPropertyName("stream")] bool Stream);

        private sealed record ChatMessage(
            [property: JsonPropertyName("role")] string Role,
            [property: JsonPropertyName("content")] string Content);

        private sealed record ChatCompletionResponse(
            [property: JsonPropertyName("choices")] IReadOnlyCollection<ChatChoice>? Choices,
            [property: JsonPropertyName("error")] AiError? Error);

        private sealed record ChatChoice(
            [property: JsonPropertyName("message")] ChatMessage? Message);

        private sealed record AiError(
            [property: JsonPropertyName("message")] string? Message);
    }
}
