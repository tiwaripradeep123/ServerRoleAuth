using CoreFeed.Core.Com.Entity.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreFeed.Core.Telemetry
{
    /// <summary>
    /// Dont Create object directly and should be singleton only
    /// </summary>
    public interface ITelemetry
    {
        void LogAsync(string log, Request request);

        void LogAsync(string log);

        /// <summary>
        /// Should only be used for packages
        /// </summary>
        /// <param name="ex"></param>
        void TrackException(Exception ex, IDictionary<string, string> properties = null, Request request = null);

        /// <summary>
        /// When code should be executed with Try and Catch with performance measure
        /// </summary>
        /// <returns></returns
        [Obsolete("Use TrackV2")]
        TResponse Track<TResponse>(Func<TResponse> action, string methodName, Request request, TelemetryArgs obejctArgs);

        /// <summary>
        /// When code should be executed with Try and Catch with performance measure
        /// </summary>
        /// <returns></returns>
        [Obsolete("Use TrackV2")]
        void Track(Action action, string methodName, Request request, TelemetryArgs obejctArgs);

        /// <summary>
        /// When code should be executed with Try and Catch with performance measure
        /// </summary>
        /// <returns></returns>
        Task<TResponse> TrackV2<TResponse>(Func<Task<TResponse>> action, string methodName, Request request, TelemetryArgs obejctArgs);

        /// <summary>
        /// Shown be used inside Cosmos provider only to measure db calls
        /// </summary>
        /// <returns></returns>
        Task<TResponse> TrackDataBaseCalls<TResponse>(Func<Task<TResponse>> action, string docName, Request request, string query, DbOperationType dbOperationType, IDictionary<string, string> props = null);

        Task TrackDataBaseCalls(Func<Task> action, string docName, Request request, string query, DbOperationType dbOperationType, IDictionary<string, string> props = null);

        /// <summary>
        /// When code should be executed with Try and Catch with performance measure
        /// </summary>
        /// <returns></returns>
        Task TrackV2(Func<Task> action, string methodName, Request request, TelemetryArgs obejctArgs);

        /// <summary>
        /// When code should be executed with Try and Catch
        /// </summary>
        /// <returns></returns>
        Task<TResponse> TryCatch<TResponse>(Func<Task<TResponse>> action, string methodName, Request request, TelemetryArgs obejctArgs, Action actionPostException = null);

        /// <summary>
        /// When code should be executed with Try and Catch
        /// </summary>
        /// <returns></returns>
        Task TryCatch(Func<Task> action, string methodName, Request request, TelemetryArgs obejctArgs, Action actionPostException = null);
    }
}