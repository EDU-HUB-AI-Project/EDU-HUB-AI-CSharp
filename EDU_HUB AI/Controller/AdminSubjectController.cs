using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;

namespace EDU_HUB_AI.Controller
{
    public class AdminSubjectController
    {
        private readonly AdminSubjectService _adminSubjectService = new AdminSubjectService();

        public async Task<ApiResponse<List<SubjectDto>>?> GetSubjects()
        {
            return await _adminSubjectService.GetSubjects();
        }

        public async Task<ApiResponse<SubjectDto>?> GetSubject(string subjectId)
        {
            return await _adminSubjectService.GetSubject(subjectId);
        }

        public async Task<ApiResponse<int>?> InsertSubject(SubjectDto subjectDto)
        {
            return await _adminSubjectService.InsertSubject(subjectDto);
        }

        public async Task<ApiResponse<int>?> UpdateSubject(string subjectId, SubjectDto subjectDto)
        {
            return await _adminSubjectService.UpdateSubject(subjectId, subjectDto);
        }

        public async Task<ApiResponse<int>?> DeleteSubject(string subjectId)
        {
            return await _adminSubjectService.DeleteSubject(subjectId);
        }

        public Action<int, int>? OnRetry
        {
            get => _adminSubjectService.OnRetry;
            set => _adminSubjectService.OnRetry = value;
        }
    }
}
