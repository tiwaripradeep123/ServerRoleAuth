using MS.ServerRoleAuthorization.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MS.ServerRoleAuthorization
{
    public class DefaultSetupConfigurations : ISetupConfigurations
    {
        private static List<RoleConfigurations> roleConfigurations { get; set; }
        private readonly ConfigurationOptions options;

        public DefaultSetupConfigurations(ConfigurationOptions options)
        {
            this.LoadConfigurations(options.ConfigData);
            this.options = options;
        }

        private void LoadConfigurations(string configData)
        {
            roleConfigurations = JsonConvert.DeserializeObject<List<RoleConfigurations>>(configData);
        }

        public IEnumerable<RoleConfigurations> RoleConfigurationsByRole(string role)
        {
            return roleConfigurations.Where(config =>
                config.Roles.FirstOrDefault(roleConfigured =>
                    roleConfigured.Equals(role, this.options.EnableIgnoreCaseMode ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) || roleConfigured.Equals(Constants.Asterisk)) != null);
        }

        public ConfigurationOptions ConfigurationOptions()
        {
            return this.options;
        }
    }
}