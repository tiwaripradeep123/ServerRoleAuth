using CoreFeed.Core.Com.Entity.App;
using CoreFeed.Core.Telemetry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CoreFeed.Core.Auth
{
    public interface IAuth
    {
        Task<Response> Execute(HttpRequestMessage request, ITelemetry telemetry, dynamic obj, Func<string, dynamic> handlerDy, Func<Func<string, object>> wf);
    }
}