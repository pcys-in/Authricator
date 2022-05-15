using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jint;
using Microsoft.Extensions.Logging;

namespace Patronum.Authricator.Authorization
{
    public class AuthorizationHandler
    {
        private readonly ConfigurationProvider _configurationProvider;
        private readonly ILogger<AuthorizationHandler> _logger;

        private ConcurrentDictionary<string, bool?> cachedPermissions = new ConcurrentDictionary<string, bool?>();
        private ConcurrentDictionary<string, object> scriptEngineCache = new ConcurrentDictionary<string, object>();


        public AuthorizationHandler(ConfigurationProvider configurationProvider,ILogger<AuthorizationHandler> logger)
        {
            _configurationProvider = configurationProvider;
            _logger = logger;
        }

        public bool? Validate(string policy, string action, object data, bool? defaultResult = false)
        {
            if (_configurationProvider.Policies == null)
                throw new AuthricatorException("Policies not initialized!");

            bool? result = null;

            if (policy.Contains(":")) // Check for Inline Policy (A/D:MODULE:SUB-MODULE-ACTION)
                result = ValidateInlinePolicy(policy, action);
            else
                result = ValidatePolicy(policy, action, data);

            return result ?? defaultResult;
        }

        /// <summary>
        /// Validate Inline Policy
        /// </summary>
        /// <param name="policy">A single Policy Example : A:MODULE:SUB_MODULE:ACTION / D:MODULE:SUB_MODULE:ACTION</param>
        /// <param name="action">The Action to check with</param>
        /// <returns></returns>
        private bool? ValidateInlinePolicy(string policy, string action)
        {
            // Generate PermissionSet for a single inline policy and Validate with action
            return ValidateForPermissionSet(GeneratePermissionSet(new[] { policy }), action);
        }


        private bool? ValidatePolicy(string policy, string action, object data)
        {
            
            if (!_configurationProvider.Policies!.ContainsKey(policy))
                throw new AuthricatorException("Policy not Found", new { policy = policy });

            Models.Authorization.PolicyConfiguration configuration = _configurationProvider.Policies![policy].Configuration;


            if (configuration.Permissions != null)
            {
                if (!cachedPermissions.ContainsKey($"{policy}->{action}"))
                    cachedPermissions[$"{policy}->{action}"] =
                        ValidateForPermissionSet(GeneratePermissionSet(configuration.Permissions.ToArray()), action);

                return cachedPermissions[$"{policy}->{action}"];
            }
            else
            {
                if (data == null)
                    throw new AuthricatorException("Data cannot be null for Script Policy Validation");

                Engine jsEngine = new Engine(options =>
                {
                    options.LimitMemory(1_000_000); // 1MB
                    options.TimeoutInterval(TimeSpan.FromSeconds(2)); // 2 Seconds
                    options.MaxStatements(1000); // 1000 Statements
                });

                try
                {
                    var evaluation = jsEngine
                        .SetValue("Logger", _logger)
                        .SetValue("Cache", scriptEngineCache)
                        .SetValue("Validate",
                            new Func<string, string, object, bool?>((_policy, _action, _data) =>
                                Validate(_policy, _action, _data)))
                        .Execute(Encoding.UTF8.GetString(Convert.FromBase64String(configuration.Script))).Invoke("evaluate", data);
                    return evaluation.ToObject() as bool?;
                }
                catch (Exception ex)
                {
                    throw new AuthricatorException("Authorization Policy Script Execution Error",  new
                    {
                        Message = ex.Message
                    });
                }

            }
        }






        /// <summary>
        /// This Method is used to Generate PermissionSetFor a policy. This is used to split as ACTION(STRING):Permission(BOOL)
        /// </summary>
        /// <param name="policies">Policies for which the Permission Set has to be Generated</param>
        /// <returns>A Dictionary with ACTION(STRING):PERMISSION(BOOL)</returns>
        /// <exception cref="AuthricatorException"></exception>
        private Dictionary<string, bool> GeneratePermissionSet(string[] policies)
        {
            Dictionary<string, bool> permissionSet = new Dictionary<string, bool>();

            foreach (string policy in policies)
            {
                // Check if the PermissionSet is in Valid Format
                if (policy.Split(":").Length != 4)
                    throw new AuthricatorException("Invalid Inline Policy Structure - Split Error",
                        new { policy = policy });

                if (!new[] { "A:", "D:" }.Contains(policy.Substring(0, 2)))
                    throw new AuthricatorException("Invalid Inline Policy Structure - Permission Error",
                        new { policy = policy });

                // Remove first letter [A]llow / [D]eny with : and save the PermissionSet -> (MODULE:SUB-MODULE:ACTION,BOOL)
                permissionSet.Add(policy.Remove(0, 2), (policy[0] == 'A'));
            }
            return permissionSet;
        }

        /// <summary>
        /// This is the Core Checking Mechanism for PermissionSet Validation
        /// * -> ALL / @ -> ANY
        /// </summary>
        /// <param name="permission">This is a Dictionary whose keys can string or with * (ALL)</param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// <exception cref="AuthricatorException"></exception>
        private bool? ValidateForPermissionSet(Dictionary<string, bool> permission, string action)
        {
            // Todo : Move this section to the LoadPolicy->ValidateAllPolicies Part
            // Only an Action can contain '@' ANY classifier
            if (permission.Keys.Any(x => x.Contains("@")))
                throw new AuthricatorException("Policy Permission cannot contain any(@) character",
                    new { permission = permission.Keys.First(x => x.Contains("@")) });

            //Split the action by :
            string[] a = action.Split(":");
            bool? result = null;

            //The Below code is where the validation begins, it checks for all the possible combinations of EXACT, @ (ANY), * (ALL)

            if (permission.ContainsKey(action)) // This checks if the action exactly matches the permission and if true returns its result
                result = permission[action];
            else if (a[1] == "@")  // MODULE:@(ANY):ACTION OR *(ALL):@(ANY):ACTION
            {
                result = permission.Keys.Count(x => x.StartsWith($"{a[0]}:") || x.StartsWith($"*:")) != 0;
            }
            else if (a[2] == "@") // MODULE:SUB-MODULE:@(ANY) OR *(ALL):*(ALL):@(ANY)
            {
                result = permission.Keys.Count(x => x.StartsWith($"{a[0]}:{a[1]}:") || x.StartsWith($"*:*:")) != 0;
            }
            else if (permission.ContainsKey($"{a[0]}:{a[1]}:*")) // MODULE:SUB-MODULE:*
            {
                result = permission[$"{a[0]}:{a[1]}:*"];
            }
            else if (permission.ContainsKey($"{a[0]}:*:*")) // MODULE:*:*
            {
                result = permission[$"{a[0]}:*:*"];
            }
            else if (permission.ContainsKey($"*:*:*")) // *(ALL):*(ALL):*(ALL) -> This is typically Super Admin use case which must have returned in a true
            {
                result = permission[$"*:*:*"];
            }

            return result;
        }
    }
}
