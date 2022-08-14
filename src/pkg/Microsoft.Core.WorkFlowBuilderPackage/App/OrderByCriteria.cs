using Newtonsoft.Json;

namespace CoreFeed.Core.Com.Entity.App
{
    public class OrderByCriteria
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool Order { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrderBy { get; set; }
    }
}