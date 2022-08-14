using Newtonsoft.Json;

namespace CoreFeed.Core.Com.Entity.App
{
    public class Error
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
    }
}