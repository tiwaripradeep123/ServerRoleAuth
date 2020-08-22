namespace MS.ServerRoleAuthorization
{
    public interface IRuleEngine
    {
        bool IsAllowed(string role, string actionName, string subActionName, string group = "*");

        bool IsAllowed(string role, string actionName, string group = "*");
    }
}