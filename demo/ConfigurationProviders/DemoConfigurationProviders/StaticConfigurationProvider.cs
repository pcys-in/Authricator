using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Patronum.Authricator;


namespace DemoConfigurationProviders
{
    public class StaticConfigurationProvider: ConfigurationProvider

    {
        public StaticConfigurationProvider(ServiceProvider provider) : base(provider.GetRequiredService<ILogger<StaticConfigurationProvider>>())
        {
            this.LoadPolicies();
        }

        private ConcurrentDictionary<string, Models.Authorization.Policy> _policies = new();

        protected override ConcurrentDictionary<string, Models.Authorization.Policy> GetPolicies()
        {
            _policies = new ConcurrentDictionary<string, Models.Authorization.Policy>();
            
            return _policies;
        }

        protected override void SavePolicy(Models.Authorization.Policy policy)
        {
            _policies[policy.PolicyName] = policy;
        }
    }

    public static class StaticConfigurationProviderExtension
    {
        public static AuthricatorBuilder AddStaticConfigurationProvider(this AuthricatorBuilder builder)
        {
            builder.Services.AddSingleton<ConfigurationProvider>(new StaticConfigurationProvider(builder.Services.BuildServiceProvider()));
            return builder;
        }
    }
}