using CoreFeed.Business.Core;
using CoreFeed.Core.Auth;
using CoreFeed.Core.KeyVault;
using CoreFeed.Core.Telemetry;
using CoreFeed.Core.WorkFlowBuilderPackage.KeyVault;
using Unity;
using Unity.Injection;

//[assembly: SuppressIldasmAttribute()]

namespace CoreFeed.Core.Funnel
{
    /// <summary>
    /// Provides registration services where T is the object for keyVault
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WorkflowRegistration : IRegistration
    {
        public static IUnityContainer Container = null;

        static WorkflowRegistration()
        {
            Container = new UnityContainer();
        }

        public WorkflowRegistration()
        {
            Registeration();
        }

        public TResolve Resolve<TResolve>()
        {
            return Container.Resolve<TResolve>();
        }

        /// <summary>
        /// Register all share components
        /// </summary>
        private void Registeration()
        {
            Container.RegisterSingleton<ITelemetry, Telemetry.Telemetry>();
            Container.RegisterSingleton<IKeyVault, KeyVault.KeyVault>(new InjectionConstructor(new object[] { ProvideConfigurationForVault() }));
            Container.RegisterSingleton<IAuth, Auth.Auth>();

            ClaimDataExtractorRegistration();
        }

        public void RegisterServices<THander, Handler>(params InjectionMember[] injectionMembers)
        {
            Container.RegisterType(typeof(THander), typeof(Handler), injectionMembers);
        }

        public void RegisterServices<THander, Handler>(string name)
        {
            Container.RegisterType(typeof(THander), typeof(Handler), name);
        }

        public void RegisterHandler<Handler>(string handlerRequest)
        {
            Container.RegisterType(typeof(IRequestHandler), typeof(Handler), handlerRequest.ToString());
        }

        public TResolve Resolve<TResolve>(string name)
        {
            return Container.Resolve<TResolve>(name);
        }

        /// <summary>
        /// For customization override ClaimDataExtractor
        /// </summary>
        public virtual void ClaimDataExtractorRegistration()
        {
            Container.RegisterSingleton<WorkFlowBuilderPackage.Funnel.IClaimDataExtractor, WorkFlowBuilderPackage.Funnel.ClaimDataExtractor>();
        }

        /// <summary>
        /// For a given request if user is authenticated, then configure additional authorizations at server for the request.
        /// You should implement class <see cref="WorkFlowBuilderPackage.Funnel.ServerRoleAuthorization"/>
        /// <para>
        /// By default, it is not configured.
        /// Make sure to throw <see cref="System.UnauthorizedAccessException "/> when user is not authorized.
        /// </para>
        /// <para>
        /// E.g. 
        /// <code>
        /// throw new <see cref="System.UnauthorizedAccessException "/>("User is not authorized for the request.");
        /// <para>
        /// 1. Implement your authorization class from <see cref="CoreFeed.Core.WorkFlowBuilderPackage.Funnel.ServerRoleAuthorization"/>
        /// </para>
        /// <para>
        /// 2. Register the new class with <see cref="CoreFeed.Core.WorkFlowBuilderPackage.Funnel.IServerRoleAuthorization"/>
        /// </para>
        /// </code>
        /// </para>
        /// </summary>
        public virtual void ServerRoleAuthorizatioRegistration()
        {
            Container.RegisterSingleton<WorkFlowBuilderPackage.Funnel.IServerRoleAuthorization, WorkFlowBuilderPackage.Funnel.ServerRoleAuthorization>();
        }

        public virtual KeyVaultConfiguration ProvideConfigurationForVault()
        {
            return new KeyVaultConfiguration();
        }
    }
}