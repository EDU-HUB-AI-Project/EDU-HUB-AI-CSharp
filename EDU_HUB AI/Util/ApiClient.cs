using EDU_HUB_AI.Config;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace EDU_HUB_AI.Util
{
    public class ApiClient
    {
        private readonly AppConfig? _appConfig = new AppConfig();
        private readonly HttpClient? _httpClient = new HttpClient();
        private string baseUrl;
        private string apiKeys;
        public ApiClient()
        {
            _appConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText("config.json"));
            baseUrl = _appConfig?.ApiSettings?.BaseUrl;
            apiKeys = _appConfig?.ApiSettings?.ApiKeys;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Add("X-Api-Secret", apiKeys); 
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json")); // 서버한테 JSON으로 응답해달라고 요청
        }

        public async Task<T> Get<T>(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url); // ResponseEntity
            response.EnsureSuccessStatusCode(); // 상태코드가 성공이 아닐 경우 예외 발생

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task<T> Post<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");  // 클라이언트에서 보내는 데이터
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(result);
        }
        public async Task<T> Put<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();  

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(result);
        }
        public async Task<T> Patch<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(result);
        }
    }
}
