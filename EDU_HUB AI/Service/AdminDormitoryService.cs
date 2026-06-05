using EDU_HUB_AI.Util;
using System.Diagnostics;
using EDU_HUB_AI.Model;

namespace EDU_HUB_AI.Service
{
    public class AdminDormitoryService
    {
        private readonly string _url = "/admin/dorm";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<DormitoryDto>>> GettDormRoomAssignStatus()
        {
            Debug.WriteLine("Called");
            return await _apiClient.Get<List<DormitoryDto>> (_url);
        }
        public async Task<ApiResponse<DormitoryDto>> GetDormRoomAssignStatusById(string dormitoryId)
        {
            Debug.WriteLine("Called");
            string url = _url + $"/{dormitoryId}";
            return await _apiClient.Get<DormitoryDto>(url);
        }
        public async Task<ApiResponse<int>> UpdateDormAssignMaxCnt(DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called");
            string url = _url + "/max-count";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCnt(string studentId,
                                                                          DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called");
            string url = _url + $"?studentId={studentId}";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }
        public async Task<ApiResponse<int>> UpdateDormCurrentCntDown(string studentId,
                                                                          DormitoryDto dormitoryDto)
        {
            Debug.WriteLine("Called");
            string url = _url + $"?studentId={studentId}";
            return await _apiClient.Patch<int>(url, dormitoryDto);
        }
    }
}
