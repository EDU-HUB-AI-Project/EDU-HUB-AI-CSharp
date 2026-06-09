using System.Diagnostics;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;

namespace EDU_HUB_AI.Service
{
    public class AdminFacilityInfoService
    {
        private readonly string _url = "/admin/facilityInfo";
        private readonly ApiClient _apiClient = new();

        public async Task<ApiResponse<List<FacilityInfoDto>>> GetFacilityList()
        {
            Debug.WriteLine("Called::GetFacilityList");
            return await _apiClient.Get<List<FacilityInfoDto>>(_url);
        }

        public async Task<ApiResponse<FacilityInfoDto>> GetFacilityById(string facilityId)
        {
            Debug.WriteLine("Called::GetFacilityById");
            return await _apiClient.Get<FacilityInfoDto>($"{_url}/{facilityId}");
        }

        public async Task<ApiResponse<int>> InsertFacility(FacilityInfoDto dto)
        {
            Debug.WriteLine("Called::InsertFacility");
            return await _apiClient.Post<int>(_url, dto);
        }

        public async Task<ApiResponse<int>> UpdateFacility(string facilityId, FacilityInfoDto dto)
        {
            Debug.WriteLine("Called::UpdateFacility");
            return await _apiClient.Put<int>($"{_url}/{facilityId}", dto);
        }

        public async Task<ApiResponse<int>> DeleteFacility(string facilityId)
        {
            Debug.WriteLine("Called::DeleteFacility");
            return await _apiClient.Delete<int>($"{_url}/{facilityId}");
        }

        // 이미지 업로드
        public async Task<ApiResponse<FacilityImageUploadDto>> UploadFacilityImage(string localFilePath)
        {
            return await _apiClient.PostMultipartAsync<FacilityImageUploadDto>($"{_url}/upload", localFilePath);
        }
    }
}
