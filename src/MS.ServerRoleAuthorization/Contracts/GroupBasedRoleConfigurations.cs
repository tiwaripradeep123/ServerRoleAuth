using System;
using System.Collections.Generic;

namespace MS.ServerRoleAuthorization.Contracts
{
    [Serializable]
    /// <summary>
    /// Defines Roles vs actions
    /// </summary>
    internal class GroupBasedRoleConfigurations
    {
        public string Group { get; set; }
        public List<RoleConfigurations> RoleConfigurations { get; set; }
    }
}