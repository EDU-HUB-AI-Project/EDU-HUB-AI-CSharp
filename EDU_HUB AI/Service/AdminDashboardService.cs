using EDU_HUB_AI.Util;
using EDU_HUB_AI.Model;
using System.Diagnostics;
using System.Text.Json;

namespace EDU_HUB_AI.Service
{
    public class AdminDashboardService
    {
        private readonly string _url = "/admin/dashboard";
        private readonly ApiClient _apiClient = new ApiClient();

        // Dictionary<string, object>에서 object는 역직렬화 대상 타입이 불명확해
        // List<>로 감싸면 System.Text.Json이 타입 추론을 포기하고 JsonException 발생
        // → object 대신 JsonElement로 명시하여 해결
        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetPrintCountByHour()
        {
            Debug.WriteLine("Called::GetPrintCountByHour");
            string url = _url + "/print-count";
            return await _apiClient.Get<List<Dictionary<string, JsonElement>>>(url);
        }

        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetPopularFeature()
        {
            Debug.WriteLine("Called::GetPopularFeature");
            string url = _url + "/popular";
            return await _apiClient.Get<List<Dictionary<string, JsonElement>>>(url);
        }

        public async Task<ApiResponse<Dictionary<string, JsonElement>>> GetAttendCount()
        {
            Debug.WriteLine("Called::GetAttendCount");
            string url = _url + "/attend-count";
            return await _apiClient.Get<Dictionary<string, JsonElement>>(url);
        }

        public async Task<ApiResponse<Dictionary<string, JsonElement>>> GetDormStats()
        {
            Debug.WriteLine("Called::GetDormStats");
            string url = _url + "/dorm-stats";
            return await _apiClient.Get<Dictionary<string, JsonElement>>(url);
        }

        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetLogTop10()
        {
            Debug.WriteLine("Called::GetLogTop10");
            string url = _url + "/kiosk-log";
            return await _apiClient.Get<List<Dictionary<string, JsonElement>>>(url);
        }

        public Action<int, int>? OnRetry
        {
            get => _apiClient.OnRetry;
            set => _apiClient.OnRetry = value;
        }
    }
}
