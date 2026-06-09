using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;

namespace EDU_HUB_AI.Controller
{
    public class AdminTransportController
    {
        private readonly AdminTransportService _service = new();

        public Task<ApiResponse<List<TransportDto>>> GetTransportList()
            => _service.GetTransportList();

        public Task<ApiResponse<TransportDto>> GetTransportById(string transportId)
            => _service.GetTransportById(transportId);

        public Task<ApiResponse<int>> InsertTransport(TransportDto dto)
            => _service.InsertTransport(dto);

        public Task<ApiResponse<int>> UpdateTransport(string transportId, TransportDto dto)
            => _service.UpdateTransport(transportId, dto);

        public Task<ApiResponse<int>> DeleteTransport(string transportId)
            => _service.DeleteTransport(transportId);
    }
}
