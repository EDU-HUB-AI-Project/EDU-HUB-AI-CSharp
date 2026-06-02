using EDU_HUB_AI.Util;
using EDU_HUB_AI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Service
{
    public class AdminAttendaceService
    {
        private readonly string _url = "/admin/attendance";
        private readonly ApiClient _apiClient = new ApiClient();
        
        public async Task<List<AttendDto>?> GetAttend()
        {
           return await _apiClient.Get<List<AttendDto>>(_url);
        }

    }
}
