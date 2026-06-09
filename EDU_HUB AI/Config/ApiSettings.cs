using System.Text.Json.Serialization;

namespace EDU_HUB_AI.Config
{
    public class ApiSettings
    {
        public string? BaseUrl { get; set; }

        [JsonPropertyName("ApiKey")]
        public string? ApiKeys { get; set; }
    }
}
