using EDU_HUB_AI.Util;
using EDU_HUB_AI.Model;
using System.Diagnostics;

namespace EDU_HUB_AI.Service
{
    public class AdminDashboardService
    {
        private readonly string _url = "/admin/dashboard";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetPrintCountByHour()
        {
            Debug.WriteLine("Called::GetPrintCountByHour");
            string url = _url + "/print-count";
            return await _apiClient.Get<List<Dictionary<string, object>>>(url);
        }

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetPopularFeature()
        {
            Debug.WriteLine("Called::GetPopularFeature");
            string url = _url + "/popular";
            return await _apiClient.Get<List<Dictionary<string, object>>>(url);
        }

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetAttendCount()
        {
            Debug.WriteLine("Called::GetAttendCount");
            string url = _url + "/attend-count";
            return await _apiClient.Get<List<Dictionary<string, object>>>(url);
        }

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetDormStats()
        {
            Debug.WriteLine("Called::GetDormStats");
            string url = _url + "/dorm-stats";
            return await _apiClient.Get<List<Dictionary<string, object>>>(url);
        }
    }
}
