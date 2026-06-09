using System.Text.Json.Serialization;

namespace EDU_HUB_AI.Model
{
    public class FacilityImageUploadDto
    {
        [JsonPropertyName("imagePath")]
        public string imagePath { get; set; } = "";
    }
}
