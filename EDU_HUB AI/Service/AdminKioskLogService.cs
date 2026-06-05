using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Diagnostics;

namespace EDU_HUB_AI.Service
{
    public class AdminKioskLogService
    {
        private readonly string _url = "/admin/kiosk-log";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<KioskLogDto>>> GetKiostLog(string? logId,
                                                                      string? action,
                                                                      string? createdAt)
        {
            Debug.WriteLine("Called::GetKiostLog");
            string url = _url + "?";
            if (logId != null) url += "logId=" + logId + "&";
            if (action != null) url += "action=" + action + "&";
            if (createdAt != null) url += "createdAt=" + createdAt + "&";
            return await _apiClient.Get<List<KioskLogDto>>(url);
        }

        public async Task<ApiResponse<int>> InsertLog(KioskLogDto kioskLogDto)
        {
            Debug.WriteLine("Called::InsertLog");
            return await _apiClient.Post<int>(_url, kioskLogDto);
        }

        public async Task<ApiResponse<int>> DeleteLog(KioskLogDto kioskLogDto)
        {
            Debug.WriteLine("Called::DeleteLog");
            return await _apiClient.Delete<int>(_url, kioskLogDto);
        }
    }
}
