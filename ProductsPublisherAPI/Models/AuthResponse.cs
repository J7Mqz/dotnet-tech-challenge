using System.Text.Json.Serialization;

namespace ProductsPublisherAPI.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
