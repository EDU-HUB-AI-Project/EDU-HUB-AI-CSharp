using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class ApiResponse<T>
    {   
        [JsonPropertyName("status")] // java에서 넘오는 변수 이름과 매핑
        public int Status { get; set; } 
        [JsonPropertyName("message")]
        public string? Message { get; set; }
        [JsonPropertyName("data")]
        public T? Data { get; set; }
    }
}
