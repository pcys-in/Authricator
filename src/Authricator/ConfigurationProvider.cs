using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Patronum.Authricator
{
    public abstract class ConfigurationProvider
    {
        private readonly ILogger<ConfigurationProvider> _logger;

        protected ConfigurationProvider(ILogger<ConfigurationProvider> logger)
        {
            this._logger = logger;

            this._logger.LogDebug("Provider Initialized");
        }

        public ConcurrentDictionary<string, Models.Authorization.Policy> Policies { get; private set; } = new();

        protected abstract ConcurrentDictionary<string, Models.Authorization.Policy> GetPolicies();
        protected abstract void SavePolicy(Models.Authorization.Policy policy);

        public void LoadPolicies()
        {
            Policies = GetPolicies();
            ValidatePolicies();
            _logger.LogDebug($"Loaded {Policies.Count} Policies");
        }

        private void ValidatePolicies()
        {
            foreach (var policy in Policies)
            {
                policy.Value.ValidateConfiguration();
            }
        }
    }
}