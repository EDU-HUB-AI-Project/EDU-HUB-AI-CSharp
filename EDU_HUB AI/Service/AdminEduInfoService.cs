using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Service
{
    public class AdminEduInfoService
    {
        private readonly string _url = "/admin/eduInfo";
        private readonly ApiClient _apiClient = new ApiClient();

        public async Task<ApiResponse<List<EduInfoDto>>?> GetEduInfos()
        {
            return await _apiClient.Get<List<EduInfoDto>>(_url);
        }

        public async Task<ApiResponse<EduInfoDto>?> GetEduInfo(string eduInfoId)
        {
            return await _apiClient.Get<EduInfoDto>(_url + "/" + eduInfoId);
        }

        public async Task<ApiResponse<int>?> InsertEduInfo(EduInfoDto eduInfoDto)
        {
            return await _apiClient.Post<int>(_url, eduInfoDto);
        }

        public async Task<ApiResponse<int>?> UpdateEduInfo(string eduInfoId, EduInfoDto eduInfoDto)
        {
            return await _apiClient.Put<int>(_url + "/" + eduInfoId, eduInfoDto);
        }

        public async Task<ApiResponse<int>?> DeleteEduInfo(string eduInfoId)
        {
            return await _apiClient.Delete<int>(_url + "/" + eduInfoId);
        }
    }
}
