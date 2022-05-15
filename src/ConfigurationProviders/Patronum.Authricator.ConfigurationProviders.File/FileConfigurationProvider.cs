using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Patronum.Authricator.ConfigurationProviders.File
{
    public class FileConfigurationProvider : Patronum.Authricator.ConfigurationProvider
    {
        public FileConfigurationProvider(ILogger<ConfigurationProvider> logger) : base(logger)
        {
        }

        protected override ConcurrentDictionary<string, Models.Authorization.Policy> GetPolicies()
        {
            throw new NotImplementedException();
        }

        protected override void SavePolicy(Models.Authorization.Policy policy)
        {
            throw new NotImplementedException();
        }
    }
}