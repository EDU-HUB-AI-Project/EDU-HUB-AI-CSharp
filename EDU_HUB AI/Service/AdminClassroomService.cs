using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Service
{
    public class AdminClassroomService
    {
        private readonly string _url = "/admin/classroom";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<ClassroomDto>>?> GetClassrooms()
        {
            return await _apiClient.Get<List<ClassroomDto>>(_url);
        }

        public async Task<ApiResponse<ClassroomDto>?> GetClassroom(string classroomId)
        {
            return await _apiClient.Get<ClassroomDto>(_url + "/" + classroomId);
        }

        public async Task<ApiResponse<int>?> UpdateClassroom(string classroomId, ClassroomDto classroomDto)
        {
            return await _apiClient.Put<int>(_url + "/" + classroomId, classroomDto);
        }

        public Action<int, int>? OnRetry
        {
            get => _apiClient.OnRetry;
            set => _apiClient.OnRetry = value;
        }
    }
}
