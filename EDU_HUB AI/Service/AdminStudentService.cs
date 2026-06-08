using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Diagnostics;

namespace EDU_HUB_AI.Service
{
    public class AdminStudentService
    {
        private readonly string _url = "/admin/student";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<StudentDto>>?> GetStudents()
        {
            Debug.WriteLine("Called :: GetStudents");
            return await _apiClient.Get<List<StudentDto>>(_url);
        }
        public async Task<ApiResponse<StudentDto>?> GetStudent(string studentId)
        {
            Debug.WriteLine("Called :: GetStudent" + " :: " + studentId);
            string url = _url + "/" + studentId;
            return await _apiClient.Get<StudentDto>(url);
        }

        public async Task<ApiResponse<int>?> InsertStudent(StudentDto studentDto)
        {
            Debug.WriteLine("Called :: InsertStudent");
            return await _apiClient.Post<int>(_url, studentDto);
        }

        public async Task<ApiResponse<int>> UpdateStudent(string studentId, StudentDto studentDto)
        {
            Debug.WriteLine("Called :: UpdateStudent :: " + studentId);
            string url = _url + "/" + studentId;
            return await _apiClient.Put<int>(url, studentDto);
        }

        public async Task<ApiResponse<int>> DeleteStudent(string studentId)
        {
            Debug.WriteLine("Called :: DeleteStudent :: " + studentId);
            string url = _url + "/" + studentId;
            return await _apiClient.Delete<int>(url);
        }
        public async Task<ApiResponse<int>?> BatchInsertStudent(List<StudentDto> students)
        {
            Debug.WriteLine("Called :: BatchInsertStudent:: ");
            string url = _url + "/batch";
            return await _apiClient.Post<int>(url, students);
        }
    }
}
