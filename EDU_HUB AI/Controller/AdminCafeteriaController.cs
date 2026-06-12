using EDU_HUB_AI.Model;
using EDU_HUB_AI.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Controller
{
    public class AdminCafeteriaController
    {
        private readonly AdminCafeteriaService _adminCafeteriaService = new AdminCafeteriaService();

        public async Task<ApiResponse<List<Dictionary<string, object>>>> GetCafeteriaSummary(string date)
        {
            Debug.WriteLine("Called::GetCafeteriaSummary");
            return await _adminCafeteriaService.GetCafeteriaSummary(date);
        }

        public async Task<ApiResponse<int>> UpdateCafeteria(string id, CafeteriaDto dto)
        {
            Debug.WriteLine("Called::UpdateCafeteria");
            return await _adminCafeteriaService.UpdateCafeteria(id, dto);
        }

        public async Task<ApiResponse<int>> DeleteCafeteria(string id)
        {
            Debug.WriteLine("Called::DeleteCafeteria");
            return await _adminCafeteriaService.DeleteCafeteria(id);
        }

        public async Task<ApiResponse<Dictionary<string, object>>> CreateCafeteriaList(List<CafeteriaDto> list)
        {
            Debug.WriteLine("Called::CreateCafeteriaList");
            return await _adminCafeteriaService.CreateCafeteriaList(list);
        }


        public async Task SaveCafeteriaList(List<CafeteriaDto> dtoList)
        {
            foreach (var dto in dtoList)
            {
                bool isDelete = dto.delYn == "Y";
                bool isNew = string.IsNullOrWhiteSpace(dto.cafeteriaId);

                if (isDelete)
                {
                    await DeleteCafeteria(dto.cafeteriaId ?? "");
                }
                else if (isNew)
                {
                    await CreateCafeteriaList(new List<CafeteriaDto> { dto });
                }
                else
                {
                    await UpdateCafeteria(dto.cafeteriaId ?? "", dto);
                }
            }
        }

        public Action<int, int>? OnRetry
        {
            get => _adminCafeteriaService.OnRetry;
            set => _adminCafeteriaService.OnRetry = value;
        }
    }
}
