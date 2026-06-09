using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;

namespace EDU_HUB_AI.Controller
{
    public class AdminFacilityInfoController
    {
        private readonly AdminFacilityInfoService _service = new();

        public Task<ApiResponse<List<FacilityInfoDto>>> GetFacilityList()
            => _service.GetFacilityList();

        public Task<ApiResponse<FacilityInfoDto>> GetFacilityById(string facilityId)
            => _service.GetFacilityById(facilityId);

        public Task<ApiResponse<int>> InsertFacility(FacilityInfoDto dto)
            => _service.InsertFacility(dto);

        public Task<ApiResponse<int>> UpdateFacility(string facilityId, FacilityInfoDto dto)
            => _service.UpdateFacility(facilityId, dto);

        public Task<ApiResponse<int>> DeleteFacility(string facilityId)
            => _service.DeleteFacility(facilityId);
    }
}
