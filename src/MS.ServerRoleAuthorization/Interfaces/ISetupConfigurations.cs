using MS.ServerRoleAuthorization.Contracts;
using System.Collections.Generic;

namespace MS.ServerRoleAuthorization
{
    internal interface ISetupConfigurations
    {
        IEnumerable<RoleConfigurations> RoleConfigurationsByRole(string role, string group);

        ConfigurationOptions ConfigurationOptions();
    }
}