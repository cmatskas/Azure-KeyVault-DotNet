using System;

namespace KeyVaultTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            var clientId = "1bbb4fd3-e903-4b00-afcb-afb9582b68b8";
            var clientSecret = "9wDEgDndxLkxvZzuVm78Ux4cUcY/sYTt3kW77N3SLSA=";
            var keyVaultUri = @"https://CmTestVault1.vault.azure.net";

            var kvService = new KeyVaultService(clientId, clientSecret, keyVaultUri);

            var keys = kvService.GetKeys().GetAwaiter().GetResult();
            foreach (var key in keys.Value)
            {
                Console.Write(key.Kid);
            }
            Console.ReadKey();
        }
    }
}
