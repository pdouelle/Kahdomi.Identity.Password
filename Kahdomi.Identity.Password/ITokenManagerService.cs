using IdentityModel.Client;

namespace Kahdomi.Identity.Password;

public interface IPasswordRequestTokenManagementService
{
    public Task<string> GetAccessTokenAsync
    (
        string clientName,
        PasswordTokenRequest passwordTokenRequest,
        TokenRenewalOption renewalOption,
        string? customTokenPath = default,
        CancellationToken cancellationToken = default
    );
}