using Newtonsoft.Json;

namespace CoreFeed.Core.Com.Entity.App
{
    public class FilterByCriteria
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool DebugMode { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool ShowTotalRecordsOnly { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int TotalRecords { get; set; } = 0;
    }
}