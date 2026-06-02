using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Controller
{
    public class AdminAttendaceController
    {
        private readonly AdminAttendaceService adminAttendaceService = new AdminAttendaceService();
        public async Task<List<AttendDto>?> GetAttend()
        {
           return await adminAttendaceService.GetAttend();
        }
    }
}
