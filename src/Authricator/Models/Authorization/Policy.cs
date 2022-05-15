namespace Patronum.Authricator;

public partial class Models
{
    public partial class Authorization
    {
        public class Policy
        {
            public string PolicyName { get; set; } = string.Empty;
            public PolicyConfiguration Configuration { get; set; } = new PolicyConfiguration();

            public void ValidateConfiguration()
            {
                // Check if Both are Null / Missing
                if (Configuration.Permissions == null && Configuration.Script == null)
                    throw new AuthricatorException("Policy is missing both Permission and Script Configurations",
                        new { policy = this });

                // Check if Both are NOT Null / Present
                if (Configuration.Permissions != null && Configuration.Script != null)
                    throw new AuthricatorException("Policy Cannot have both Permission and Script Configurations",
                        new { policy = this });
            }
        }

        public class PolicyConfiguration
        {
            public List<string>? Permissions { get; set; } = null;
            public string? Script { get; set; } = null;
        }
    }
    
}