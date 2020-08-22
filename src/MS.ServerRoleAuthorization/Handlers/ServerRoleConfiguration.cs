using MS.ServerRoleAuthorization.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MS.ServerRoleAuthorization
{
    internal class DefaultSetupConfigurations : ISetupConfigurations
    {
        private static List<GroupBasedRoleConfigurations> roleConfigurations { get; set; }
        private readonly ConfigurationOptions options;

        public DefaultSetupConfigurations(ConfigurationOptions options)
        {
            Contract.Requires(options != null, $"Parameter {nameof(options)} must be not null {options}.");

            this.LoadConfigurations(options.ConfigData);
            this.options = options;
        }

        private void LoadConfigurations(string configData)
        {
            roleConfigurations = JsonConvert.DeserializeObject<List<GroupBasedRoleConfigurations>>(configData);
        }

        public IEnumerable<RoleConfigurations> RoleConfigurationsByRole(string role, string group)
        {
            return roleConfigurations.FirstOrDefault(gr => gr.Group.Equals(group, options.EnableIgnoreCaseMode ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))?.RoleConfigurations.Where(config =>
                config.Roles.FirstOrDefault(roleConfigured =>
                    roleConfigured.Equals(role, options.EnableIgnoreCaseMode ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal) || roleConfigured.Equals(Constants.Asterisk, StringComparison.CurrentCulture)) != null);
        }

        public ConfigurationOptions ConfigurationOptions()
        {
            return this.options;
        }
    }
}