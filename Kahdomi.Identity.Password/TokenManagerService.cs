using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;

namespace Kahdomi.Identity.Password;

public sealed class PasswordRequestTokenManagementService : IPasswordRequestTokenManagementService
{
    private readonly HttpClient _httpClient;
    private readonly IDistributedCache _cache;

    public PasswordRequestTokenManagementService(
        HttpClient httpClient,
        IDistributedCache cache)
    {
        _httpClient = httpClient;
        _cache = cache;
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
            await _cache.SetStringAsync(clientName, accessToken, cancellationToken);
        }

        return accessToken;
    }
}