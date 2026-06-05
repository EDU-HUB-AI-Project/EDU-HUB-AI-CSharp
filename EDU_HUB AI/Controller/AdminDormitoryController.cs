using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System.Diagnostics;

namespace EDU_HUB_AI.Controller
{
    public class AdminDormitoryController
    {
        private readonly AdminDormitoryService _adminDormitoryService = new AdminDormitoryService();

        public async Task<ApiResponse<List<DormitoryDto>>> GettDormRoomAssignStatus()
        {
            Debug.WriteLine("Called::GettDormRoomAssignStatus");
            return await _adminDormitoryService.GettDormRoomAssignStatus();
        }

        public async Task<ApiResponse<DormitoryDto>> GetDormRoomAssignStatusById(string dormitoryId)
        {
            Debug.WriteLine("Called::GetDormRoomAssignStatusById");
            return await _adminDormitoryService.GetDormRoomAssignStatusById(dormitoryId);
        }

        public async Task<ApiResponse<int>> UpdateDormAssignMaxCnt(DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormAssignMaxCnt");
            return await _adminDormitoryService.UpdateDormAssignMaxCnt(dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCnt(string studentId, DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormCurrentCnt");
            return await _adminDormitoryService.UpdateDormCurrentCnt(studentId, dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCntDown(string studentId,
                                                                          DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormCurrentCntDown");
            return await _adminDormitoryService.UpdateDormCurrentCntDown(studentId, dormitoryDto);
        }
    }
}
