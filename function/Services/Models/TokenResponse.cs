using Newtonsoft.Json;

namespace Function.Model
{
    public class TokenResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_in")]
        public int Expired { get; set; }
    }
}