using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Midas.Application.Common.Interfaces.Repositories;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
        private readonly string _apiKey;
        private readonly string _endpointUrl;

        public AiAssistantService(HttpClient httpClient, IOptions<AiAssistantSettings> settings, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _apiKey = configuration["AiSettings:ApiKey"]!;
            _endpointUrl = "https://api.groq.com/openai/v1/chat/completions";
        }

        public async Task<string> GenerateDescription(string type, string name, string category, List<string>? items = null)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                throw new InvalidOperationException("AiSettings:ApiKey is not configured.");
            }

            string prompt = "";

            if (type.Equals("product", StringComparison.OrdinalIgnoreCase))
            {
                prompt = $"Ти — копірайтер інтернет-магазину. Напиши короткий, привабливий опис для товару. Назва: {name}, Категорія: {category}. Пиши українською мовою, лаконічно, максимум 3-4 речення. Тільки опис, без зайвих привітань.";
            }
            else if (type.Equals("order", StringComparison.OrdinalIgnoreCase) && items != null)
            {
                string itemsList = string.Join(", ", items);
                prompt = $"Сформуй короткий технічний коментар/супровідний опис для логістики на основі складу замовлення. У замовленні є такі товари: {itemsList}. Напиши українською мовою, сухо та по справі, що саме відправляється і на що звернути увагу.";
            }
            var requestBody = new
            {
                model = "openai/gpt-oss-120b",
                messages = new[] { new { role = "user", content = prompt }  },
                temperature = 0.7
            };
            var request = new HttpRequestMessage(HttpMethod.Post, _endpointUrl)
            {
                Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                throw new Exception("Помилка при запиті до сервісу штучного інтелекту.");

            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseString);

            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content?.Trim() ?? string.Empty;
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

            var response = await _httpClient.PostAsJsonAsync(_settings.BaseUrl, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: cancellationToken);
                var message = errorResult?.Error?.Message ?? response.ReasonPhrase ?? "AI service request failed.";
                throw new InvalidOperationException(message);
            }

            var result = await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken: cancellationToken);

            return result?.Message?.Content?.Trim() ?? "AI сервіс не повернув рекомендації.";
        }

        private sealed record ChatCompletionRequest(
            [property: JsonPropertyName("model")] string Model,
            [property: JsonPropertyName("messages")] IReadOnlyCollection<ChatMessage> Messages,
            [property: JsonPropertyName("stream")] bool Stream);

        private sealed record ChatMessage(
            [property: JsonPropertyName("role")] string Role,
            [property: JsonPropertyName("content")] string Content);

        private sealed record ChatCompletionResponse(
            [property: JsonPropertyName("message")] ChatMessage? Message,
            [property: JsonPropertyName("error")] AiError? Error);

        private sealed record AiError(
            [property: JsonPropertyName("message")] string? Message);
    }
}