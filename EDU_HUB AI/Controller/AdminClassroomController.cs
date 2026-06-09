using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;

namespace EDU_HUB_AI.Controller
{
    public class AdminClassroomController
    {
        private readonly AdminClassroomService _adminClassroomService = new AdminClassroomService();

        public async Task<ApiResponse<List<ClassroomDto>>?> GetClassrooms() {
            return await _adminClassroomService.GetClassrooms();
        }

        public async Task<ApiResponse<ClassroomDto>?> GetClassroom(string classroomId)
        {
            return await _adminClassroomService.GetClassroom(classroomId);
        }

        public async Task<ApiResponse<int>?> UpdateClassroom(string classroomId, ClassroomDto classroomDto)
        {
            return await _adminClassroomService.UpdateClassroom(classroomId, classroomDto);
        }

        public Action<int, int>? OnRetry
        {
            get => _adminClassroomService.OnRetry;
            set => _adminClassroomService.OnRetry = value;
        }
    }
}
