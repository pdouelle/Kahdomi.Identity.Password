using IdentityModel.Client;
using Microsoft.Extensions.DependencyInjection;

namespace TotalEnergies.Acquisition.Common.Authentication;

public static class PasswordTokenManagementServiceCollectionExtensions
{
    public static IHttpClientBuilder AddPasswordRequestTokenHandler(
        this IHttpClientBuilder builder,
        string clientName,
        PasswordTokenRequest passwordTokenRequest) =>
        builder.AddHttpMessageHandler(provider =>
        {
            var passwordAuthenticationService = provider.GetRequiredService<IPasswordRequestTokenManagementService>();
            
            return new PasswordRequestTokenHandler(passwordAuthenticationService, clientName, passwordTokenRequest);
        });
    
    public static void AddPasswordTokenManagement(this IServiceCollection services)
    {
        services.AddTransient<PasswordRequestTokenHandler>();
        services.AddScoped<IPasswordRequestTokenManagementService, PasswordRequestTokenManagementService>();
    }
}