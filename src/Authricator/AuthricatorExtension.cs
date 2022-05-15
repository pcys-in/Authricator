using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patronum.Authricator.Authorization;

namespace Patronum.Authricator;

public static class AuthricatorExtension
{
    public static void InitializeAuthricator(this IServiceCollection services, Action<AuthricatorBuilder> configure)
    {
        var logger = services.BuildServiceProvider().GetRequiredService<ILogger<Authricator>>();
        logger.LogDebug("Authricator Initializing");

        configure(new AuthricatorBuilder(services));
        services.AddScoped<AuthorizationHandler>();
    }
}

public class AuthricatorBuilder
{
    public readonly IServiceCollection Services;

    public AuthricatorBuilder(IServiceCollection services)
    {
        Services = services;
    }
}

