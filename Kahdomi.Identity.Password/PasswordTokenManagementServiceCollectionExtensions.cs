using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Kahdomi.Identity.Password;

public static class PasswordTokenManagementServiceCollectionExtensions
{
    public static IHttpClientBuilder AddPasswordRequestTokenHandler(
        this IHttpClientBuilder builder,
        string clientName,
        PasswordTokenRequest passwordTokenRequest,
        string? customTokenPath = default) =>
        builder.AddHttpMessageHandler(provider =>
        {
            var passwordAuthenticationService = provider.GetRequiredService<ITokenManagerService>();
            
            return new PasswordRequestTokenHandler(passwordAuthenticationService, clientName, passwordTokenRequest, customTokenPath);
        });
    
    public static void AddPasswordTokenManagement(this IServiceCollection services)
    {
        services.AddTransient<PasswordRequestTokenHandler>();
        services.AddScoped<ITokenManagerService, TokenManagerService>();
    }
}