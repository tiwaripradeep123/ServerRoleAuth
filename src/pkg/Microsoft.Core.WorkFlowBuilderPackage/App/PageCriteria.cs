using Newtonsoft.Json;

namespace CoreFeed.Core.Com.Entity.App
{
    public class PageCriteria
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        /// <summary>
        /// This should be set to True to enable page
        /// </summary>
        public bool EnablePage { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]

        /// <summary>
        /// If Value is true it means next page has some more values: UI dont need to send this. UI
        /// will get this value in Reponse
        /// </summary>
        public bool HasMoreData { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool IsAdditionalFilter { get; set; } = false;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int PageSize { get; set; } = 10;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Skip { get; set; } = 0;
    }
}