using Microsoft.ApplicationInsights;
using CoreFeed.Core.Com.Entity.App;
using CoreFeed.Core.KeyVault;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
//[assembly: SuppressIldasmAttribute()]

namespace CoreFeed.Core.Telemetry
{
    public enum DbOperationType
    {
        GetByQuery,
        GetByDocumentId,
        RemoveByDocumentId,
        UpdateByDocument,
        GetCount,
        GetCountByFilter
    }

    /// <summary>
    /// Dont Create obejct directly and should be singleton only
    /// </summary>
    public class Telemetry : ITelemetry
    {
        private readonly IKeyVault _keyVault = null;
        private readonly TelemetryClient _telemetryClient = null;

        public Telemetry()
        {
        }

        /// <summary>
        /// Dont Create obejct directly and should be singleton only
        /// </summary>
        /// <param name="instrumentationKey"></param>
        public Telemetry(IKeyVault keyVault)
        {
            _keyVault = keyVault;

#pragma warning disable CS0618 // Type or member is obsolete
            _telemetryClient = new TelemetryClient()
            {
                InstrumentationKey = _keyVault.GetValue("InstrumentationKey")
            };
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void LogAsync(string log, Request request)
        {
            LogAsync(log, request.GetProperties());
        }

        public void LogAsync(string log)
        {
            _telemetryClient.TrackTrace
                ("LogAsync",
                new Dictionary<string, string>() { { "Message", log } });
        }

        public TResponse Track<TResponse>(Func<TResponse> action, string methodName, Request request, TelemetryArgs obejctArgs)
        {
            obejctArgs = (obejctArgs ?? TelemetryArgs.DefaultInstance);

            try
            {
                var respose = action.Invoke();
                return respose;
            }
            catch (Exception ex)
            {
                var properties = request?.GetProperties();
                properties?.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, request.GetProperties());

                if (obejctArgs.ThrowException)
                {
                    throw;
                }
                else
                {
                    return default(TResponse);
                }
            }
        }

        public void Track(Action action, string methodName, Request request, TelemetryArgs obejctArgs)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                var properties = request.GetProperties();
                properties.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, properties);

                if ((obejctArgs ?? TelemetryArgs.DefaultInstance).ThrowException)
                {
                    throw;
                }
            }
        }

        public async Task TrackDataBaseCalls(Func<Task> action, string docName, Request request, string query, DbOperationType dbOperationType, IDictionary<string, string> props = null)
        {
            var propertis = (request?.GetProperties()) ?? new Dictionary<string, string>();
            propertis.Add("IsInvokerCall", "false");
            propertis.Add("IsDbCall", "true");
            propertis.Add("Collection", docName ?? string.Empty);
            propertis.Add("Query_Raw", query ?? string.Empty);
            propertis.Add("Query_Computed", (query ?? string.Empty).Replace("root", docName));
            propertis.Add("DbOperationType", dbOperationType.ToString());

            if (props != null && props.Any())
            {
                foreach (var p in props)
                {
                    propertis.Add(p.Key, p.Value);
                }
            }

            try
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                await action.Invoke();
                watch.Stop();

                propertis.Add("DurationInMs", watch.ElapsedMilliseconds.ToString());

                _telemetryClient.TrackMetric($"{docName}", Convert.ToDouble(watch.ElapsedMilliseconds), propertis);
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, propertis);

                throw ex;
            }
        }

        public async Task<TResponse> TrackDataBaseCalls<TResponse>(Func<Task<TResponse>> action, string docName, Request request, string query, DbOperationType dbOperationType, IDictionary<string, string> props = null)
        {
            var propertis = (request?.GetProperties()) ?? new Dictionary<string, string>();
            propertis.Add("IsInvokerCall", "false");
            propertis.Add("IsDbCall", "true");
            propertis.Add("Collection", docName ?? string.Empty);
            propertis.Add("Query_Raw", query ?? string.Empty);
            propertis.Add("Query_Computed", (query ?? string.Empty).Replace("root", docName));
            propertis.Add("DbOperationType", dbOperationType.ToString());

            if (props != null && props.Any())
            {
                foreach (var p in props)
                {
                    propertis.Add(p.Key, p.Value);
                }
            }

            try
            {
                Stopwatch watch = new Stopwatch();

                watch.Start();
                var respose = await action.Invoke();
                watch.Stop();

                propertis.Add("DurationInMs", watch.ElapsedMilliseconds.ToString());

                _telemetryClient.TrackMetric($"{docName}", Convert.ToDouble(watch.ElapsedMilliseconds), propertis);

                return respose;
            }
            catch (Exception ex)
            {
                _telemetryClient.TrackException(ex, propertis);

                throw ex;
            }
        }

        public void TrackException(Exception ex, IDictionary<string, string> properties = null, Request request = null)
        {
            var propertis = (request?.GetProperties()) ?? new Dictionary<string, string>();
            if (properties != null && properties.Any())
            {
                foreach (var p in properties)
                {
                    propertis.Add(p.Key, p.Value);
                }
            }
            _telemetryClient.TrackException(ex, properties);
        }

        public async Task<TResponse> TrackV2<TResponse>(Func<Task<TResponse>> action, string methodName, Request request, TelemetryArgs obejctArgs)
        {
            obejctArgs = (obejctArgs ?? TelemetryArgs.DefaultInstance);

            try
            {
                LogAsync($"Executing : {methodName}", request);
                Stopwatch watch = new Stopwatch();

                watch.Start();
                var respose = await action.Invoke();
                watch.Stop();

                var propertis = request?.GetProperties();
                propertis?.Add(nameof(methodName), methodName);
                propertis?.Add("DurationInMs", watch.ElapsedMilliseconds.ToString());
                propertis?.Add("IsInvokerCall", obejctArgs.InvokerCall.ToString());

                _telemetryClient.TrackMetric($"{methodName}", Convert.ToDouble(watch.ElapsedMilliseconds), propertis);
                LogAsync($"Executed : {methodName}", request);

                return respose;
            }
            catch (Exception ex)
            {
                var properties = request?.GetProperties();
                properties?.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, request.GetProperties());

                if (obejctArgs.ThrowException)
                {
                    throw;
                }
                else
                {
                    return default(TResponse);
                }
            }
        }

        public async Task TrackV2(Func<Task> action, string methodName, Request request, TelemetryArgs obejctArgs)
        {
            try
            {
                LogAsync($"Executing : {methodName}", request);

                Stopwatch watch = new Stopwatch();
                watch.Start();

                await action.Invoke();

                watch.Start();
                _telemetryClient.TrackMetric($"{methodName}", Convert.ToDouble(watch.ElapsedMilliseconds), request.GetProperties());

                LogAsync($"Executed : {methodName}", request);
            }
            catch (Exception ex)
            {
                var properties = request.GetProperties();
                properties.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, properties);

                if ((obejctArgs ?? TelemetryArgs.DefaultInstance).ThrowException)
                {
                    throw;
                }
            }
        }

        public async Task<TResponse> TryCatch<TResponse>(Func<Task<TResponse>> action, string methodName, Request request, TelemetryArgs obejctArgs, Action actionPostException = null)
        {
            obejctArgs = (obejctArgs ?? TelemetryArgs.DefaultInstance);

            try
            {
                LogAsync($"Executing : {methodName}", request);

                var respose = await action.Invoke();

                var propertis = request?.GetProperties();
                propertis?.Add(nameof(methodName), methodName);
                LogAsync($"Executed : {methodName}", propertis);

                return respose;
            }
            catch (Exception ex)
            {
                var properties = request?.GetProperties();
                properties?.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, properties);

                if (actionPostException != null)
                {
                    actionPostException.Invoke();
                }

                if (obejctArgs.ThrowException)
                {
                    throw ex;
                }
                else
                {
                    return default(TResponse);
                }
            }
        }

        public async Task TryCatch(Func<Task> action, string methodName, Request request, TelemetryArgs obejctArgs, Action actionPostException = null)
        {
            try
            {
                LogAsync($"Executing : {methodName}", request);
                await (action.Invoke());
                LogAsync($"Executed : {methodName}", request);
            }
            catch (Exception ex)
            {
                var properties = request.GetProperties();
                properties.Add(nameof(methodName), methodName);

                _telemetryClient.TrackException(ex, properties);

                if (actionPostException != null)
                {
                    actionPostException.Invoke();
                }

                if ((obejctArgs ?? TelemetryArgs.DefaultInstance).ThrowException)
                {
                    throw;
                }
            }
        }

        private void LogAsync(string log, IDictionary<string, string> properties)
        {
            _telemetryClient.TrackTrace(log, properties);
        }
    }
}