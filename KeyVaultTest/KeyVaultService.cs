using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace KeyVaultTest
{
    public class KeyVaultService
    {
        private string clientId;
        private string clientSecret;
        private string keyVaultUri;
        private static ClientCredential clientCredential;
        private readonly KeyVaultClient keyVaultClient;

        public KeyVaultService(string clientId, string clientSecret, string keyVaultUri)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.keyVaultUri = keyVaultUri;
            clientCredential = new ClientCredential(clientId, clientSecret);
            keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetAccessToken), GetHttpClient());
        }

        public async Task<KeyBundle> CreateKey(string keyName)
        {
            var keyBundle = GenerateKeyBundle();
            var tags = GetDefaultKeyTags();

            return await keyVaultClient.CreateKeyAsync(keyVaultUri, keyName, keyBundle.Key.Kty, keyAttributes: keyBundle.Attributes, tags: tags);
        }

        public async Task<KeyBundle> CreateKey(string keyName, Dictionary<string, string> tags)
        {
            var keyBundle = GenerateKeyBundle();
            return await keyVaultClient.CreateKeyAsync(keyVaultUri, keyName, keyBundle.Key.Kty, keyAttributes: keyBundle.Attributes, tags: tags);
        }

        public async Task<KeyBundle> CreateKey(string keyName, KeyBundle keyBundle, Dictionary<string, string> tags)
        {
            return await keyVaultClient.CreateKeyAsync(keyVaultUri, keyName, keyBundle.Key.Kty, keyAttributes: keyBundle.Attributes, tags: tags);
        }

        public async Task<ListKeysResponseMessage> GetKeys(int? maxResults = null)
        {
            return  await keyVaultClient.GetKeysAsync(keyVaultUri, maxResults);
        }

        public async Task<KeyBundle> UpdateKey(string keyName, KeyAttributes keyAttributes)
        {
            return await keyVaultClient.UpdateKeyAsync(keyVaultUri, keyName, attributes: keyAttributes);
        }

        public async Task<KeyBundle> GetKey(string keyName, string keyVersion = null)
        {
            return keyVersion != string.Empty
                ? await keyVaultClient.GetKeyAsync(keyVaultUri, keyName, keyVersion)
                : await keyVaultClient.GetKeyAsync(keyVaultUri, keyName);
        }

        public async void DeleteKey(string keyName)
        {
            var keyBundle = await keyVaultClient.DeleteKeyAsync(keyVaultUri, keyName);
        }


        private KeyBundle GenerateKeyBundle()
        {
            return new KeyBundle
            {
                Key = new JsonWebKey()
                {
                    Kty = JsonWebKeyType.EllipticCurve
                },
                Attributes = new KeyAttributes()
                {
                    Enabled = true,
                    Expires = UnixEpoch.FromUnixTime(int.MaxValue),
                    NotBefore = UnixEpoch.FromUnixTime(0),
                }
            };
        }

        private Dictionary<string, string> GetDefaultKeyTags()
        {
            return new Dictionary<string, string> { { "use", "demo Key Vault operations" }, { "app", "SampleKeyVault" } };
        }

        public static async Task<string> GetAccessToken(string authority, string resource, string scope)
        {
            var context = new AuthenticationContext(authority, TokenCache.DefaultShared);
            var result = await context.AcquireTokenAsync(resource, clientCredential);

            return result.AccessToken;
        }

        private static HttpClient GetHttpClient()
        {
            return (HttpClientFactory.Create(new InjectHostHeaderHttpMessageHandler()));
        }
    }
}
