using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System.Diagnostics;
using System.Text.Json;

namespace EDU_HUB_AI.Controller
{
    public class AdminDashBoardController
    {
        private readonly AdminDashboardService _adminDashBoardService = new AdminDashboardService();

        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetPrintCountByHour()
        {
            Debug.WriteLine("Called::GetPrintCountByHour");
            return await _adminDashBoardService.GetPrintCountByHour();
        }
        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetPopularFeature()
        {
            Debug.WriteLine("Called::GetPopularFeature");
            return await _adminDashBoardService.GetPopularFeature();
        }

        public async Task<ApiResponse<Dictionary<string, JsonElement>>> GetAttendCount()
        {
            Debug.WriteLine("Called::GetAttendCount");
            return await _adminDashBoardService.GetAttendCount();
        }

        public async Task<ApiResponse<Dictionary<string, JsonElement>>> GetDormStats()
        {
            Debug.WriteLine("Called::GetDormStats");
            return await _adminDashBoardService.GetDormStats();
        }
        
        public async Task<ApiResponse<List<Dictionary<string, JsonElement>>>> GetLogTop10()
        {
            Debug.WriteLine("Called::GetLogTop10");
            return await _adminDashBoardService.GetLogTop10();
        }
        public Action<int, int>? OnRetry
        {
            get => _adminDashBoardService.OnRetry;
            set => _adminDashBoardService.OnRetry = value;
        }
    }
}
