using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Service
{
    public class AdminCafeteriaService
    {
        private readonly string _url = "/admin/cafeteria";
        private readonly ApiClient _apiClient = new ApiClient();

        // 날짜별 식단 요약 조회
        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetCafeteriaSummary(string date)
        {
            Debug.WriteLine("Called::GetCafeteriaSummary");
            string url = _url + $"/{date}";
            return await _apiClient.Get<List<Dictionary<string, object>>>(url);
        }

        // 수정
        public async Task<ApiResponse<int>> UpdateCafeteria(string id, CafeteriaDto dto)
        {
            Debug.WriteLine("Called::UpdateCafeteria");
            string url = _url + $"/{id}";
            return await _apiClient.Put<int>(url, dto);
        }

        // 삭제
        public async Task<ApiResponse<int>> DeleteCafeteria(string id)
        {
            Debug.WriteLine("Called::DeleteCafeteria");
            string url = _url + $"/{id}";
            return await _apiClient.Delete<int>(url);
        }

        public async Task<ApiResponse<Dictionary<string, object>>> CreateCafeteriaList(List<CafeteriaDto> list)
        {
            Debug.WriteLine("Called::CreateCafeteriaLit");
            return await _apiClient.Post<Dictionary<string, object>>(_url, list);
        }
    }
}
