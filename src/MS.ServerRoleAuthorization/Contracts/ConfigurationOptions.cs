using Unity;

namespace MS.ServerRoleAuthorization.Contracts
{
    public class ConfigurationOptions
    {
        public ConfigurationOptions(IUnityContainer container, string configData)
        {
            this.ConfigData = configData;
            this.Container = container;
        }

        public ConfigurationOptions WithIgnoreCaseMode(bool enableIgnoreCaseMode)
        {
            var option = Clone(this);
            option.EnableIgnoreCaseMode = enableIgnoreCaseMode;

            return option;
        }

        public IUnityContainer Container { get; private set; }

        public string ConfigData { get; private set; }
        public bool EnableIgnoreCaseMode { get; private set; } = true;

        private ConfigurationOptions Clone(ConfigurationOptions source)
        {
            return new ConfigurationOptions(source.Container, source.ConfigData)
            {
                EnableIgnoreCaseMode = source.EnableIgnoreCaseMode
            };
        }
    }
}