using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoreFeed.Core.Com.Entity.App
{
    public class ExecutionContext
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContextId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ParentId { get; set; }

        /// <summary>
        /// This should be set at API level and not anywhere else
        /// </summary>
        public string TrackingId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContextData { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        /// <summary>
        /// The DPFT object 'D'ata'P'rovider'For''T'oken.
        /// <para>
        /// Data updated using <seealso cref="CoreFeed.Core.WorkFlowBuilderPackage.Funnel.IClaimDataExtractor"/> can be extracted for each call using <seealso cref="ExecutionContext.DPFT"/>
        /// Please note, this is end to end encrypted channel over protocol. Does not provide guaranty to lost data.
        /// </para>
        /// </summary>
        public Lazy<Dictionary<string, string>> DPFT { get; set; }
    }
}