using System;

namespace IdpServer.Domain.Entity
{
    public class ClientSecret
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public int ClientId { get; set; }
        public string ObfuscatedSecret { get; set; }

        public ClientSecret() 
        {

        }

        public ClientSecret(string clientName, string clientSecret, string clientSecretHash)
        {
            Description = $"{clientName} secret";
            Type = "SharedSecret";
            Value = clientSecretHash;
            ObfuscatedSecret = $"{clientSecret.Substring(0, 3)}****{clientSecret.Substring(clientSecret.Length - 3)}";
        }
    }
}
