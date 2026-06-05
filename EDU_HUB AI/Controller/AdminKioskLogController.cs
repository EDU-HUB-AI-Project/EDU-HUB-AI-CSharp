using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Controller
{
    public class AdminKioskLogController
    {
        private readonly AdminKioskLogService _adminKioskLogService = new AdminKioskLogService();

        public async Task<ApiResponse<List<KioskLogDto>>> GetKiostLog(string? logId,
                                                                      string? action,
                                                                      string? createdAt)
        {
            Debug.WriteLine("Called::GetKiostLog");
            return await _adminKioskLogService.GetKiostLog(logId, action, createdAt);
        }
        public async Task<ApiResponse<int>> InsertLog(KioskLogDto kioskLogDto)
        {
            Debug.WriteLine("Called::InsertLog");
            return await _adminKioskLogService.InsertLog(kioskLogDto);
        }
        public async Task<ApiResponse<int>> DeleteLog(KioskLogDto kioskLogDto)
        {
            Debug.WriteLine("Called::DeleteLog");
            return await _adminKioskLogService.DeleteLog(kioskLogDto);
        }
    }
}
