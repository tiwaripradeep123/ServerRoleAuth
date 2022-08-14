using Microsoft.Azure.KeyVault;
using CoreFeed.Core.WorkFlowBuilderPackage.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//[assembly: SuppressIldasmAttribute()]

namespace CoreFeed.Core.KeyVault
{
    public class KeyVault : IKeyVault
    {
        private Dictionary<string, KeyVaultObject> _mappings = null;

        public KeyVault(KeyVaultConfiguration keyVaultConfiguration)
        {
            KeyVaultConfiguration = keyVaultConfiguration;
            LoadKeys();
        }

        public KeyVaultConfiguration KeyVaultConfiguration { get; }

        public byte[] ExtractCertificate(string certKey, bool exportKey = false)
        {
            try
            {
                var vaultAddress = KeyVaultConfiguration.KeyVaultUrl;

                var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));

                if (!exportKey)
                {
                    //append vault address
                    certKey = $"{vaultAddress}/certificates/{certKey}";
                    var secret = keyVaultClient.GetCertificateAsync(certKey).ConfigureAwait(false).GetAwaiter().GetResult();
                    return secret.Cer;
                }
                else
                {
                    certKey = $"{vaultAddress}secrets/{certKey}";
                    var secret = keyVaultClient.GetSecretAsync(certKey).ConfigureAwait(false).GetAwaiter().GetResult();
                    return Convert.FromBase64String(secret.Value);
                }

            }
            catch (Exception){ return new byte[0]; }
        }

        public string GetValue(string key)
        {
            var response = "";

            KeyVaultObject obj = null;
            if (_mappings.TryGetValue(key, out obj))
            {
                response = obj.KeyValue;
            }

            return response;
        }

        private async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var clientId = KeyVaultConfiguration.ClientId;
            var clientSecret = KeyVaultConfiguration.ClientSecret;

            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);

            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }

        private void LoadAdditionalKeys(KeyVaultClient keyVaultClient, string vaultAddress)
        {
            var prefix = KeyVaultConfiguration.Convertor.EnvKey;

            foreach (string foo in KeyVaultConfiguration.KeysToExtract)
            {
                if (KeyVaultConfiguration.Convertor.Action.Invoke(foo))
                {
                    var secret = keyVaultClient.GetSecretAsync(vaultAddress, KeyVaultConfiguration.Convertor.KeyRename.Invoke(foo.ToString())).ConfigureAwait(false).GetAwaiter().GetResult();

                    _mappings.Add(foo, new KeyVaultObject { DestinationSource = DestinationSource.KeyVault, KeyValue = secret.Value });
                }
            }
        }

        private void LoadKeys()
        {
            var vaultAddress = KeyVaultConfiguration.KeyVaultUrl;

            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken));
            var secret = keyVaultClient.GetSecretAsync(vaultAddress, KeyVaultConfiguration.KeyToExtract).ConfigureAwait(false).GetAwaiter().GetResult();

            _mappings = JsonConvert.DeserializeObject<Dictionary<string, KeyVaultObject>>(secret.Value);
            LoadAdditionalKeys(keyVaultClient, vaultAddress);
        }
    }
}