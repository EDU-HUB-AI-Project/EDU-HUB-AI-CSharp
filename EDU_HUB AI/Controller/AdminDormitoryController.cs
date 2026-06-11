using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using EDU_HUB_AI.Util;
using System.Diagnostics;

namespace EDU_HUB_AI.Controller
{
    public class AdminDormitoryController
    {
        private readonly AdminDormitoryService _adminDormitoryService = new AdminDormitoryService();
        private readonly ApiClient _apiClient = new ApiClient();
        public async Task<ApiResponse<List<DormAssignDto>>> GetDormAssign()
        {
            Debug.WriteLine("Called::GetDormAssign");
            return await _adminDormitoryService.GetDormAssign();
        }

        public async Task<ApiResponse<List<DormitoryDto>>> GetCmbDorm()
        {
            Debug.WriteLine("Called::GetCmbDorm");
            return await _adminDormitoryService.GetCmbDorm();
        }

        public async Task<ApiResponse<List<DormInOutDto>>> GetDormWaiting()
        {
            Debug.WriteLine("Called::GetDormWaiting");
            return await _adminDormitoryService.GetDormWaiting();
        }

        public async Task<ApiResponse<List<DormInOutDto>>> GetDormIn()
        {
            Debug.WriteLine("Called::GetDormIn");
            return await _adminDormitoryService.GetDormIn();
        }

        public async Task<ApiResponse<List<DormInOutDto>>> GetDormOut()
        {
            Debug.WriteLine("Called::GetDormOut");
            return await _adminDormitoryService.GetDormOut();
        }
        public async Task<ApiResponse<List<DormitoryDto>>> GetDormRoomAssignStatus(string? dormitoryId=null)
        {
            Debug.WriteLine("Called::GettDormRoomAssignStatus");
            return await _adminDormitoryService.GetDormRoomAssignStatus(dormitoryId);
        }

        public async Task<ApiResponse<DormitoryDto>> GetDormRoomAssignStatusById(string dormitoryId)
        {
            Debug.WriteLine("Called::GetDormRoomAssignStatusById");
            return await _adminDormitoryService.GetDormRoomAssignStatusById(dormitoryId);
        }

        public async Task<ApiResponse<int>> UpdateDormId(string studentId, DormAssignDto dormAssignDto)
        {
            Debug.WriteLine("Called::UpdateDormId");
            return await _adminDormitoryService.UpdateDormId(studentId, dormAssignDto);
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

        public Action<int, int>? OnRetry
        {
            get => _adminDormitoryService.OnRetry;
            set => _adminDormitoryService.OnRetry = value;
        }
    }
}
