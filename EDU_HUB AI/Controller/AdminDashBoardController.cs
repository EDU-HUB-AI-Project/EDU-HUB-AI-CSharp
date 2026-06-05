using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System.Diagnostics;

namespace EDU_HUB_AI.Controller
{
    public class AdminDashBoardController
    {
        private readonly AdminDashboardService _adminDashBoardService = new AdminDashboardService();

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetPrintCountByHour()
        {
            Debug.WriteLine("Called::");
            return await _adminDashBoardService.GetPrintCountByHour();
        }
        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetPopularFeature()
        {
            Debug.WriteLine("Called::");
            return await _adminDashBoardService.GetPopularFeature();
        }

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetAttendCount()
        {
            Debug.WriteLine("Called::");
            return await _adminDashBoardService.GetAttendCount();
        }

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetDormStats()
        {
            Debug.WriteLine("Called::");
            return await _adminDashBoardService.GetDormStats();
        }
    }
}
