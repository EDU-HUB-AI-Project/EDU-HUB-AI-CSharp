using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Service
{
    public class AdminKioskOperationalLogService
    {
        private readonly string _url = "/admin/kiosk-op-log";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<KioskOperationalLogDto>>> GetOpLog(string studentId,
                                                                              string dorm,
                                                                              string createdAt)
        {
            Debug.WriteLine("Called::");
            string url = _url + "?";
            if (studentId != null) url += "studentId=" + studentId + "&";
            if (dorm != null) url += "dorm=" + dorm + "&";
            if (createdAt != null) url += "createdAt=" + createdAt + "&";
            return await _apiClient.Get<List<KioskOperationalLogDto>>(url);
        }

        public async Task<ApiResponse<int>> InsertOpLog (KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::");
            string url = _url + "/op-log";
            return await _apiClient.Post<int>(url, kioskOperationalLogDto);
        }

        public async Task<ApiResponse<int>> InsertDormOutLog(KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::");
            string url = _url + "/dorm-op";
            return await _apiClient.Post<int>(url, kioskOperationalLogDto);
        }
        public async Task<ApiResponse<int>> DeleteLog(KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::");
            return await _apiClient.Delete<int>(_url, kioskOperationalLogDto);
        }
    }
}
