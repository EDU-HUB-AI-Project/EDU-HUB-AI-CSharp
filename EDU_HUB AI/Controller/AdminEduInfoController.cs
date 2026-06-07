using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Controller
{
    public class AdminEduInfoController
    {
        private readonly AdminEduInfoService _adminEduInfoService = new AdminEduInfoService();

        public async Task<ApiResponse<List<EduInfoDto>>?> GetEduInfos()
        {
            return await _adminEduInfoService.GetEduInfos();
        }

        public async Task<ApiResponse<EduInfoDto>?> GetEduInfo(string eduInfoId)
        {
            return await _adminEduInfoService.GetEduInfo(eduInfoId);
        }

        public async Task<ApiResponse<int>?> InsertEduInfo(EduInfoDto eduInfoDto)
        {
            return await _adminEduInfoService.InsertEduInfo(eduInfoDto);
        }
       
        public async Task<ApiResponse<int>?> UpdateEduInfo(string eduInfoId, EduInfoDto eduInfoDto)
        {
            return await _adminEduInfoService.UpdateEduInfo(eduInfoId, eduInfoDto);
        }

        public async Task<ApiResponse<int>?> DeleteEduInfo(string eduInfoId)
        {
            return await _adminEduInfoService.DeleteEduInfo(eduInfoId);
        }
    }
}
