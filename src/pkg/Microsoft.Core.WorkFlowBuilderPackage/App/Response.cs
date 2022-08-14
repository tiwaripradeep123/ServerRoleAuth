using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreFeed.Core.Com.Entity.App
{
    public class Response
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; } = new List<Error>();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ExecutionContext ExecutionContext { get; set; } = new ExecutionContext();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public FilterByCriteria FilterByCriteria { get; set; } = new FilterByCriteria();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Header { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public PageCriteria PageCriteria { get; set; } = new PageCriteria();

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StatusCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StatusMessage { get; set; }
    }
}