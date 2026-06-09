using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System.Diagnostics;

namespace EDU_HUB_AI.Controller
{
    public class AdminStudentController
    {
        private readonly AdminStudentService _adminStudentService = new AdminStudentService();

        public async Task<ApiResponse<List<StudentDto>>?> GetStudents()
        {
            Debug.WriteLine("Called :: GetStudents");
            return await _adminStudentService.GetStudents();
        }

        public async Task<ApiResponse<StudentDto>?> GetStudent(string studentId)
        {
            Debug.WriteLine("Called :: GetStudent :: " + studentId);
            return await _adminStudentService.GetStudent(studentId);
        }

        public async Task<ApiResponse<int>?> InsertStudent(StudentDto studentDto)
        {
            Debug.WriteLine("Called :: InsertStudent");
            return await _adminStudentService.InsertStudent(studentDto);
        }

        public async Task<ApiResponse<int>?> UpdateStudent(string studentId, StudentDto studentDto)
        {
            Debug.WriteLine("Called :: UpdateStudent :: " + studentId);
            return await _adminStudentService.UpdateStudent(studentId, studentDto);
        }

        public async Task<ApiResponse<int>?> DeleteStudent(string studentId)
        {
            Debug.WriteLine("Called :: DeleteStudent :: " + studentId);
            return await _adminStudentService.DeleteStudent(studentId);
        }
        public async Task<ApiResponse<int>?> BatchInsertStudent(List<StudentDto> students)
        {
            Debug.WriteLine("Called :: BatchInsertStudent");
            return await _adminStudentService.BatchInsertStudent(students);
        }

        public Action<int, int>? OnRetry
        {
            get => _adminStudentService.OnRetry;
            set => _adminStudentService.OnRetry = value;
        }
    }
}
