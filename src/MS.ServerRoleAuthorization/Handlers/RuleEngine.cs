using MS.ServerRoleAuthorization.Contracts;
using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MS.ServerRoleAuthorization
{
    internal class RuleEngine : IRuleEngine
    {
        public RuleEngine(ISetupConfigurations setupConfigurations, IRuleHandler ruleHandler)
        {
            this.setupConfigurations = setupConfigurations;
            this.ruleHandler = ruleHandler;
        }

        private readonly ISetupConfigurations setupConfigurations;
        private readonly IRuleHandler ruleHandler;

        public virtual bool IsAllowed(string role, string actionName, string subActionName, string group = "*")
        {
            Contract.Requires(string.IsNullOrWhiteSpace(role), $"Parameter {nameof(role)} must be not null {role}.");
            Contract.Requires(string.IsNullOrWhiteSpace(actionName), $"Parameter {nameof(actionName)} must be not null {actionName}.");
            Contract.Requires(string.IsNullOrWhiteSpace(subActionName), $"Parameter {nameof(subActionName)} must be not null {subActionName}.");

            return EvaluateRuleConditions(role, actionName, subActionName, group);
        }

        public virtual bool IsAllowed(string role, string actionName, string group = "*")
        {
            Contract.Requires(string.IsNullOrWhiteSpace(role), $"Parameter {nameof(role)} must be not null {role}.");
            Contract.Requires(string.IsNullOrWhiteSpace(actionName), $"Parameter {nameof(actionName)} must be not null {actionName}.");

            return EvaluateRuleConditions(role, actionName, Constants.Asterisk, group);
        }

        private bool EvaluateRuleConditions(string role, string actionName, string subActionName, string group)
        {
            var roleBsedConfigurations = setupConfigurations.RoleConfigurationsByRole(role, group);

            if (roleBsedConfigurations == null || !roleBsedConfigurations.Any())
            {
                throw new NotSupportedException($"Provided value='{role}' for param {nameof(role)} is not configured or supported. Please setup configurations for this role.");
            }

            return ruleHandler.ExecuteRule(roleBsedConfigurations, (actionName, subActionName));
        }
    }
}