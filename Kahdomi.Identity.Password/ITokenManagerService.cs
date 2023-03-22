using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Kahdomi.Identity.Password;

public interface ITokenManagerService
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