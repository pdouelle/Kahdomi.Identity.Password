using System.Net;
using IdentityModel.Client;

namespace TotalEnergies.Acquisition.Common.Authentication;

public sealed class PasswordRequestTokenHandler : DelegatingHandler
{
    private readonly IPasswordRequestTokenManagementService _accessTokenManagementService;
    private readonly string _clientName;
    private readonly PasswordTokenRequest _passwordTokenRequest;

    public PasswordRequestTokenHandler(
        IPasswordRequestTokenManagementService accessTokenManagementService,
        string clientName,
        PasswordTokenRequest passwordTokenRequest)
    {
        _accessTokenManagementService = accessTokenManagementService;
        _clientName = clientName;
        _passwordTokenRequest = passwordTokenRequest;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await SetTokenAsync(request, false, cancellationToken);
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode is HttpStatusCode.Unauthorized)
        {
            response.Dispose();
            
            await SetTokenAsync(request, true, cancellationToken);
            return await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
   
    private async Task SetTokenAsync(HttpRequestMessage request, bool forceRenewal, CancellationToken cancellationToken)
    {
        var token = await _accessTokenManagementService.GetAccessTokenAsync(_clientName, _passwordTokenRequest, forceRenewal, cancellationToken);

        request.SetBearerToken(token);
    }
}