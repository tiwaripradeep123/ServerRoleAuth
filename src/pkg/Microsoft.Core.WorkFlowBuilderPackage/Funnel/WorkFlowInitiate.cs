using CoreFeed.Core.Auth;
using CoreFeed.Business.Core;
using CoreFeed.Core.Com.Entity.App;
using CoreFeed.Core.Telemetry;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreFeed.Core.Funnel
{
    /// <summary>
    /// Funnel creation with identifier with auth support for all pipelines
    /// </summary>
    public class WorkFlowInitiator
    {
        private static IAuth auth = null;
        private static IRegistration registration = null;
        private static ITelemetry telemetry = null;

        public static void Start(IRegistration registrationv)
        {
            registration = registrationv;
            telemetry = registration.Resolve<ITelemetry>();
            auth = registration.Resolve<IAuth>();
        }

        /// <summary>
        /// Initiate work flow for the authorized http request
        /// </summary>       
        /// <param name="request"></param>
        /// <param name="dto">if any</param>
        /// <returns></returns>
        public static async Task<Response> InitiateAsync<IWorkFlowDataProvider>(HttpRequestMessage request, object dto)
        {
            return await auth.Execute(
                request,
                telemetry,
                dto,
                (rt) => registration.Resolve<IRequestHandler>(rt),
                () => (dTo) => registration.Resolve<IWorkFlowDataProvider>(dTo));
        }
    }
}