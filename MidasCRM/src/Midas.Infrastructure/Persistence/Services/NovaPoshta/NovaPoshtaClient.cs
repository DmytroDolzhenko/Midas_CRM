using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Midas.Infrastructure.Persistence.Services.NovaPoshta
{
    public class NovaPoshtaClient : INovaPoshtaClient
    {
        private const int MaxRetryAttempts = 3;
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
            Guid companyId,
            string modelName,
            string calledMethod,
            TRequest properties,
            CancellationToken ct)
        {
            var integration = await _context.UserIntegrations
                .FirstOrDefaultAsync(x => x.CompanyId == companyId && x.Provider == "novaposhta", ct)
                ?? throw new Exception("Nova Poshta integration not found for this company.");

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

            NovaPoshtaApiResponse<TResponse>? result = null;
            Exception? lastException = null;

            for (var attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    using var response = await _httpClient.PostAsJsonAsync(string.Empty, requestBody, jsonOptions, ct);
                    response.EnsureSuccessStatusCode();

                    result = await response.Content.ReadFromJsonAsync<NovaPoshtaApiResponse<TResponse>>(jsonOptions, ct);
                    lastException = null;
                    break;
                }
                catch (Exception ex) when (IsTransient(ex) && attempt < MaxRetryAttempts)
                {
                    lastException = ex;
                    var backoff = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    await Task.Delay(backoff, ct);
                }
            }

            if (lastException is not null && result is null)
            {
                throw new Exception("Nova Poshta request failed after retries.", lastException);
            }

            if (result == null || !result.Success)
            {
                var errors = result?.Errors != null ? string.Join(", ", result.Errors) : "Unknown NP Error";
                throw new Exception($"Nova Poshta Error: {errors}");
            }

            return result.Data;
        }

        private static bool IsTransient(Exception ex)
            => ex is HttpRequestException
            || ex is IOException
            || ex is TaskCanceledException;
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
