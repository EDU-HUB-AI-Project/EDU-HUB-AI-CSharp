using EDU_HUB_AI.Config;
using EDU_HUB_AI.Model;
using System.DirectoryServices;
using System.Net.Http;
using System.Reflection.Metadata;
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

        public async Task<ApiResponse<T>> Get<T>(string url) // Task: js의 promise와 같은 역할
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url); // ResponseEntity
            //response.EnsureSuccessStatusCode(); // 상태코드가 성공이 아닐 경우 예외 발생
            // 상태코드를 분리하지 않음으로서 위 코드는 필요 없어짐
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(json);
        }

        public async Task<ApiResponse<T>> Post<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");  // 클라이언트에서 보내는 데이터
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(result);
        }
        public async Task<ApiResponse<T>> Put<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(result);
        }
        public async Task<ApiResponse<T>> Patch<T>(string? url, Object? body)
        {
            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PatchAsync(url, content);

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(result);
        }

        public async Task Delete(string? url)
        {
            await _httpClient.DeleteAsync(url);
        }

        public async Task<ApiResponse<T>> Delete<T>(string? url)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(url);
            //response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ApiResponse<T>>(result);
        }
    }
}
