using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Service
{
    public class AdminSubjectService      
    {
        private readonly string _url = "/admin/subject";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<SubjectDto>>?> GetSubjects()
        {
            return await _apiClient.Get<List<SubjectDto>>(_url);
        }

        public async Task<ApiResponse<SubjectDto>?> GetSubject(string subjectId)
        {
            return await _apiClient.Get<SubjectDto>(_url + "/" + subjectId);
        }

        public async Task<ApiResponse<int>?> InsertSubject(SubjectDto subjectDto)
        {
            return await _apiClient.Post<int>(_url, subjectDto);
        }

        public async Task<ApiResponse<int>?> UpdateSubject(string subjectId, SubjectDto subjectDto)
        {
            return await _apiClient.Put<int>(_url + "/" + subjectId, subjectDto);
        }

        public async Task<ApiResponse<int>?> DeleteSubject(string subjectId)
        {
            return await _apiClient.Delete<int>(_url + "/" + subjectId) ;
        }

        public Action<int, int>? OnRetry
        {
            get => _apiClient.OnRetry;
            set => _apiClient.OnRetry = value;
        }
    }
}
