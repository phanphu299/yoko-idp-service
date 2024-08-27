using Newtonsoft.Json;

namespace Function.Model
{
     public class AuthenticationResultDto
    {
        public string Result { get; set; } = "deny";
        [JsonProperty("is_superuser")]
        public bool IsSuperUser { get; set; } = false; //false for now
    }
}
