using EDU_HUB_AI.Util;
using System.Diagnostics;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Service
{
    public class AdminDormitoryService
    {
        private readonly string _url = "/admin/dorm";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<DormAssignDto>>> GetDormAssign()
        {
            Debug.WriteLine("Called::GetDormAssign");
            return await _apiClient.Get<List<DormAssignDto>>(_url);
        }

        public async Task<ApiResponse<List<DormitoryDto>>> GetCmbDorm()
        {
            Debug.WriteLine("Called::GetCmbDorm");
            string url = _url + $"/combo-box";
            return await _apiClient.Get<List<DormitoryDto>>(url);
        }
        public async Task<ApiResponse<List<DormInOutDto>>> GetDormWaiting()
        {
            Debug.WriteLine("Called::GetDormWaiting");
            string url = _url + $"/waiting";
            return await _apiClient.Get<List<DormInOutDto>>(url);
        }
        public async Task<ApiResponse<List<DormInOutDto>>> GetDormIn()
        {
            Debug.WriteLine("Called::GetDormIn");
            string url = _url + $"/check-in";
            return await _apiClient.Get<List<DormInOutDto>>(url);
        }
        public async Task<ApiResponse<List<DormInOutDto>>> GetDormOut()
        {
            Debug.WriteLine("Called::GetDormOut");
            string url = _url + $"/check-out";
            return await _apiClient.Get<List<DormInOutDto>>(url);
        }

        public async Task<ApiResponse<List<DormitoryDto>>> GettDormRoomAssignStatus()
        {
            Debug.WriteLine("Called::GettDormRoomAssignStatus");
            string url = _url + $"/assign-status";
            return await _apiClient.Get<List<DormitoryDto>> (url);
        }
        public async Task<ApiResponse<DormitoryDto>> GetDormRoomAssignStatusById(string dormitoryId)
        {
            Debug.WriteLine("Called::GetDormRoomAssignStatusById");
            string url = _url + $"/{dormitoryId}";
            return await _apiClient.Get<DormitoryDto>(url);
        }

        public async Task<ApiResponse<int>> UpdateDormId(string studentId, DormAssignDto dormAssignDto)
        {
            Debug.WriteLine("Called::UpdateDormId");
            string url = _url + $"/{studentId}";
            return await _apiClient.Patch<int>(url, dormAssignDto);
        }

        public async Task<ApiResponse<int>> UpdateDormAssignMaxCnt(DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormAssignMaxCnt");
            string url = _url + "/max-count";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCnt(string studentId,
                                                                          DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormCurrentCnt");
            string url = _url + $"/current-count?studentId={studentId}";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCntDown(string studentId,
                                                                          DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called::UpdateDormCurrentCntDown");
            string url = _url + $"/current-down?studentId={studentId}";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }

        public Action<int, int>? OnRetry
        {
            get => _apiClient.OnRetry;
            set => _apiClient.OnRetry = value;
        }
    }
}
