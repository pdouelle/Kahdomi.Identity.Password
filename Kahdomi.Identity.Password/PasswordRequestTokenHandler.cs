using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Kahdomi.Identity.Password;

public sealed class PasswordRequestTokenHandler : DelegatingHandler
{
    private readonly ITokenManagerService _tokenManagerService;
    private readonly string _clientName;
    private readonly PasswordTokenRequest _passwordTokenRequest;
    private readonly string? _customTokenPath;

    public PasswordRequestTokenHandler(
        ITokenManagerService tokenManagerService,
        string clientName,
        PasswordTokenRequest passwordTokenRequest,
        string? customTokenPath)
    {
        if (string.IsNullOrWhiteSpace(clientName))
        {
            throw new ArgumentException("Client name cannot be null or empty.", nameof(clientName));
        }
        
        _tokenManagerService = tokenManagerService ?? throw new ArgumentNullException(nameof(tokenManagerService));
        _clientName = clientName;
        _passwordTokenRequest = passwordTokenRequest ?? throw new ArgumentNullException(nameof(passwordTokenRequest));
        _customTokenPath = customTokenPath;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await SetAuthorizationHeaderAsync(request, TokenRenewalOption.NoRenewal, cancellationToken);
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            response.Dispose();

            await SetAuthorizationHeaderAsync(request, TokenRenewalOption.ForceRenewal, cancellationToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task SetAuthorizationHeaderAsync(HttpRequestMessage request, TokenRenewalOption renewalOption, CancellationToken cancellationToken)
    {
        var token = await _tokenManagerService.GetAccessTokenAsync(_clientName, _passwordTokenRequest, renewalOption, _customTokenPath, cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}