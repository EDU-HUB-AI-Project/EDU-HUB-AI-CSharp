using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Controller
{
    public class AdminKioskOperationalLogController
    {
        private readonly AdminKioskOperationalLogService _adminKioskOperationalLogService = new();

        public async Task<ApiResponse<List<KioskOperationalLogDto>>> GetOpLog(string studentId,
                                                                              string dorm,
                                                                              string createdAt)
        {
            Debug.WriteLine("Called::GetOpLog");
            return await _adminKioskOperationalLogService.GetOpLog(studentId, dorm, createdAt);
        }
        public async Task<ApiResponse<int>> InsertOpLog(KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::InsertOpLog");
            return await _adminKioskOperationalLogService.InsertOpLog(kioskOperationalLogDto);
        }
        public async Task<ApiResponse<int>> InsertDormOutLog(KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::InsertDormOutLog");
            return await _adminKioskOperationalLogService.InsertDormOutLog(kioskOperationalLogDto);
        }
        public async Task<ApiResponse<int>> DeleteLog(KioskOperationalLogDto kioskOperationalLogDto)
        {
            Debug.WriteLine("Called::DeleteLog");
            return await _adminKioskOperationalLogService.DeleteLog(kioskOperationalLogDto);
        }
    }
}
