using System;
using System.Collections.Generic;

namespace CoreFeed.Core.WorkFlowBuilderPackage.KeyVault
{
    /// <summary>
    /// Initialize  Azure keyVault configuration for service configuration
    /// </summary>
    public class KeyVaultConfiguration
    {
        /// <summary>
        /// Client Id for Secure call
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// ClientSecret for Secure call
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// URL for Secure call
        /// </summary>
        public string KeyVaultUrl { get; set; }

        /// <summary>
        /// List of names to be downloaded.
        /// </summary>
        public List<string> KeysToExtract { get; set; }

        /// <summary>
        /// Single Key to download all data.
        /// 
        /// </summary>
        public string KeyToExtract { get; set; }

        /// <summary>
        /// Additional customization in case you need convert-or.
        /// </summary>
        public (Func<string, bool> Action, Func<string, string> KeyRename, string EnvKey) Convertor { get; set; }
    }
}