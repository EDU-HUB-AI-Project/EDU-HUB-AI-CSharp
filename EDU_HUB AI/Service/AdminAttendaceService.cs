using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Diagnostics;


namespace EDU_HUB_AI.Service
{
    public class AdminAttendaceService
    {
        private readonly string _url = "/admin/attendance";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<AttendDto>>?> GetAttend()
        {
            return await _apiClient.Get<List<AttendDto>>(_url);
        }

        public async Task<ApiResponse<Dictionary<string, object>>?> InsertAttend(AttendDto attendDto)
        {
            return await _apiClient.Post<Dictionary<string, object>>(_url, attendDto);
        }
        public async Task<ApiResponse<int>> InsertAttendList(List<AttendDto> attendDtoList)
        {
            string url = _url + "/list";
            return await _apiClient.Post<int>(url, attendDtoList);
        }

        public async Task<ApiResponse<int>> UpdateAttendMsg(string studentId, AttendDto attendDto)
        {
            string url = _url + $"/{studentId}";
            return await _apiClient.Patch<int>(url, attendDto);
        }

        public async Task<ApiResponse<object>?> DeleteAttend(string attendId)
        {
            string url = _url + $"/{attendId}";
            return await _apiClient.Delete<object>(url);
        }

        public Action<int, int>? OnRetry
        {
            get => _apiClient.OnRetry;
            set => _apiClient.OnRetry = value;
        }
    }
}
