using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Diagnostics;


namespace EDU_HUB_AI.Service
{
    public class AdminAttendaceService
    {
        private readonly string _url = "/admin/attendance";
        private readonly ApiClient _apiClient = new ApiClient();
        // data의 반환 타입에 따라 설정
        public async Task<ApiResponse<List<AttendDto>>?> GetAttend()
        {
            Debug.WriteLine("Called::GetAttend");
            string url = _url;
            //if (studentId != null) url += "studentId=" + studentId + "&";
            //if (eduId != null) url += "eduId=" + eduId + "&";
            //if (attendDate != null) url += "attendDate=" + attendDate + "&";
            //if (status != null) url += "status=" + status + "&";
            return await _apiClient.Get<List<AttendDto>>(url);
        }

        public async Task<ApiResponse<Dictionary<string, object>>?> InsertAttend(AttendDto attendDto)
        {
            Debug.WriteLine("Called::InsertAttend");
            return await _apiClient.Post<Dictionary<string, object>>(_url, attendDto);
        }
        public async Task<ApiResponse<int>> InsertAttendList(List<AttendDto> attendDtoList)
        {
            Debug.WriteLine("Called::InsertAttend");
            string url = _url + "/list";
            return await _apiClient.Post<int>(url, attendDtoList);
        }

        public async Task<ApiResponse<int>> UpdateAttendMsg(string studentId, AttendDto attendDto)
        {
            Debug.WriteLine("Called::UpdateAttendMsg");
            string url = _url + $"/{studentId}";
            return await _apiClient.Patch<int>(url, attendDto);
        }

        public async Task<ApiResponse<object>?> DeleteAttend(string attendId)
        {
            Debug.WriteLine("Called::DeleteAttend");
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
