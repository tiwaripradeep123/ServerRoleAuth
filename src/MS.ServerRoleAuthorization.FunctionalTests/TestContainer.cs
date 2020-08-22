using MS.ServerRoleAuthorization.Contracts;
using Unity;

namespace MS.ServerRoleAuthorization.FunctionalTests
{
    public class TestContainer
    {
        public TestContainer(string configData)
        {
            Container = new UnityContainer();
            this.RegisterServerRoleComponents(configData);
        }

        private void RegisterServerRoleComponents(string configData)
        {
            var options = new ConfigurationOptions(Container, configData)
                .WithIgnoreCaseMode(true);
            RegisterRules.SetupConfiguration(options);
        }

        public UnityContainer Container { get; }
    }
}