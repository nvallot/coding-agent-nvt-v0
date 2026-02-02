using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Shared.Models;
using System.Net;
using System.Text.Json;

namespace FAP_57.SendPOSupplier.Services;

public class LucyApiService : ILucyApiService
{
    private readonly ILogger<LucyApiService> _logger;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public LucyApiService(
        ILogger<LucyApiService> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _baseUrl = configuration["LucyApiBaseUrl"]
            ?? throw new InvalidOperationException("LucyApiBaseUrl not configured");

        // Configuration HTTP Client
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<LucyUserResponse?> GetUserByIdAsync(string userId, string correlationId)
    {
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(r => r.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        "[{CorrelationId}] Lucy API retry {RetryCount} after {Delay}ms. Status: {StatusCode}",
                        correlationId, retryCount, timeSpan.TotalMilliseconds, 
                        outcome.Result?.StatusCode ?? (HttpStatusCode)0);
                });

        try
        {
            var response = await retryPolicy.ExecuteAsync(async () =>
            {
                return await _httpClient.GetAsync($"/api/users/{userId}");
            });

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning(
                    "[{CorrelationId}] Lucy API returned 404 for user {UserId}",
                    correlationId, userId);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<LucyUserResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            _logger.LogDebug(
                "[{CorrelationId}] Lucy API returned user: {GivenName} {SurName}",
                correlationId, userInfo?.GivenName, userInfo?.SurName);

            return userInfo;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex,
                "[{CorrelationId}] Failed to retrieve user {UserId} from Lucy API",
                correlationId, userId);
            throw;
        }
    }
}
