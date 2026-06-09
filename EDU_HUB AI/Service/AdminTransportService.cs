using System.Diagnostics;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Service
{
    public class AdminTransportService
    {
        private readonly string _url = "/admin/transport";
        private readonly ApiClient _apiClient = new();

        public async Task<ApiResponse<List<TransportDto>>> GetTransportList()
        {
            Debug.WriteLine("Called::GetTransportList");
            return await _apiClient.Get<List<TransportDto>>(_url);
        }

        public async Task<ApiResponse<TransportDto>> GetTransportById(string transportId)
        {
            Debug.WriteLine("Called::GetTransportById");
            return await _apiClient.Get<TransportDto>($"{_url}/{transportId}");
        }

        public async Task<ApiResponse<int>> InsertTransport(TransportDto dto)
        {
            Debug.WriteLine("Called::InsertTransport");
            return await _apiClient.Post<int>(_url, dto);
        }

        public async Task<ApiResponse<int>> UpdateTransport(string transportId, TransportDto dto)
        {
            Debug.WriteLine("Called::UpdateTransport");
            return await _apiClient.Put<int>($"{_url}/{transportId}", dto);
        }

        public async Task<ApiResponse<int>> DeleteTransport(string transportId)
        {
            Debug.WriteLine("Called::DeleteTransport");
            return await _apiClient.Delete<int>($"{_url}/{transportId}");
        }
    }
}
