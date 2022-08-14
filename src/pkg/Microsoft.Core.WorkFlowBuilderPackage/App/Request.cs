using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace CoreFeed.Core.Com.Entity.App
{
    /// Add this entity in all Request parameter </summary>
    public class Request
    {
        [Obsolete("Dont use constructor")]
        public Request()
        {
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Func<string, object> DTO { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExecutionContext ExecutionContext { get; set; } = new ExecutionContext();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        /// <summary>
        /// Not supported currently
        /// </summary>
        public FilterByCriteria FilterByCriteria { get; set; } = new FilterByCriteria();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        /// <summary>
        /// Not supported currently
        /// </summary>
        public OrderByCriteria OrderByCriteria { get; set; } = new OrderByCriteria();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PageCriteria PageCriteria { get; set; } = new PageCriteria();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JObject RequestParam { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestSubType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestType { get; set; }
    }
}