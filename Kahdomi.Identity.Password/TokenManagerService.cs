using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Kahdomi.Identity.Password;

public sealed class TokenManagerService : ITokenManagerService
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TokenManagerService> _logger;

    public TokenManagerService(HttpClient httpClient, IDistributedCache cache, ILogger<TokenManagerService> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(
        string clientName,
        PasswordTokenRequest passwordTokenRequest,
        TokenRenewalOption renewalOption,
        string? customTokenPath = default,
        CancellationToken cancellationToken = default)
    {
        var accessToken = await _cache.GetStringAsync(clientName, cancellationToken);

        if (renewalOption == TokenRenewalOption.ForceRenewal || string.IsNullOrEmpty(accessToken))
        {
            TokenResponse tokenResponse = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest, cancellationToken);
            accessToken = customTokenPath is not null ? tokenResponse.Json.TryGetString(customTokenPath) : tokenResponse.AccessToken;
            
            if (string.IsNullOrEmpty(accessToken))
            {
                _logger.LogWarning("Failed to retrieve access token for client '{ClientName}'. Please verify the path to the custom token ({CustomTokenPath}). The default path is 'access_token'", clientName, customTokenPath);
                throw new InvalidOperationException($"Failed to retrieve access token for client '{clientName}'. Please verify the path to the custom token ({customTokenPath}). The default path is 'access_token'");
            }
            
            await _cache.SetStringAsync(clientName, accessToken, cancellationToken);
        }

        return accessToken;
    }
}