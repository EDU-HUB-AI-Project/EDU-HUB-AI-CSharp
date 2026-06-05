using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using EDU_HUB_AI.Util;
using System.Diagnostics;

namespace EDU_HUB_AI.Controller
{
    public class AdminAttendaceController
    {
        private readonly AdminAttendaceService _adminAttendaceService = new AdminAttendaceService();

        public async Task<ApiResponse<List<AttendDto>>?> GetAttend(string? studentId, string? eduId, string? attendDate, string? status)
        {
            Debug.WriteLine("Called::GetAttend");
            return await _adminAttendaceService.GetAttend(studentId, eduId, attendDate, status);
        }

        public async Task<ApiResponse<Dictionary<string, object>>?> InsertAttend(AttendDto attendDto)
        {
            Debug.WriteLine("Called::InsertAttend");
            return await _adminAttendaceService.InsertAttend(attendDto);
        }
        public async Task<ApiResponse<int>> InsertAttendList(List<AttendDto> attendDtoList)
        {
            Debug.WriteLine("Called::InsertAttendList");
            return await _adminAttendaceService.InsertAttendList(attendDtoList);
        }
        public async Task<ApiResponse<int>> UpdateAttendMsg(string studentId, AttendDto attendDto)
        {
            Debug.WriteLine("Called::UpdateAttendMsg");
            return await _adminAttendaceService.UpdateAttendMsg(studentId, attendDto);
        }
        public async Task DeleteAttend(string attendId)
        {
            Debug.WriteLine("Called::DeleteAttend");
            await _adminAttendaceService.DeleteAttend(attendId);
        }
    }
}
