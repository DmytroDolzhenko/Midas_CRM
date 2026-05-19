using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaClient : INovaPoshtaClient
    {
        private readonly HttpClient _httpClient;
        private readonly IApplicationDbContext _context;
        private readonly IEncryptionService _encryption;
        public NovaPoshtaClient(HttpClient httpClient, IApplicationDbContext context, IEncryptionService encryption)
        {
            _httpClient = httpClient;
            _context = context;
            _encryption = encryption;
        }
        public async Task<List<TResponse>> ExecuteAsync<TRequest, TResponse>(
            Guid userId,
            string modelName,
            string calledMethod,
            TRequest properties,
            CancellationToken ct)
        {
            var integration = await _context.UserIntegrations
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Provider == "novaposhta", ct)
                ?? throw new Exception("Nova Poshta integration not found for this user.");

            string apiKey = _encryption.Decrypt(integration.EncryptedAccessToken);

            var requestBody = new NovaPoshtaRequest<TRequest>
            {
                ApiKey = apiKey,
                ModelName = modelName,
                CalledMethod = calledMethod,
                MethodProperties = properties
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                PropertyNameCaseInsensitive = true
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", requestBody, ct);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<NovaPoshtaApiResponse<TResponse>>(jsonOptions, ct);

            if (result == null || !result.Success)
            {
                var errors = result?.Errors != null ? string.Join(", ", result.Errors) : "Unknown NP Error";
                throw new Exception($"Nova Poshta Error: {errors}");
            }

            return result.Data;
        }
    }
    public class NovaPoshtaApiResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
    public class NovaPoshtaRequest<T>
    {
        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; } = null!;

        [JsonPropertyName("modelName")]
        public string ModelName { get; set; } = null!;

        [JsonPropertyName("calledMethod")]
        public string CalledMethod { get; set; } = null!;

        [JsonPropertyName("methodProperties")]
        public T MethodProperties { get; set; } = default!;
    }
}
