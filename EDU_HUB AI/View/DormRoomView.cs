using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.View
{
    public partial class DormRoomView : UserControl
    {
        private List<DormitoryDto> _all = new();
        private List<DormitoryDto> _pageItems = new();
        private readonly AdminDormitoryController _adminDormitoryController = new();
        public DormRoomView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;
            pageHeader1.SyncClicked += (_, _) => LoadAndRender(1);
            pagination1.PageChanged += (_, page) => RenderPage(page);
            btnSearch.Click += BtnSearch_Click;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            await LoadCmb();
            await LoadAndRender(1);
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(grid, 0);
            bodyPanel.Controls.SetChildIndex(actionPanel, 1);
            bodyPanel.Controls.SetChildIndex(pagination1, 2);
        }

        private async Task<List<DormitoryDto>> LoadData()
        {
            string? dormitoryId = cmbDormRoom.SelectedIndex > 0 ? cmbDormRoom.SelectedValue.ToString() : null;
            var res = await _adminDormitoryController.GetDormRoomAssignStatus(dormitoryId);
            return res?.Data ?? new List<DormitoryDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            _adminDormitoryController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
            try
            {
                _all = await LoadData();
                RenderPage(page);
            }
            catch (ApiException ex)
            {
                MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminDormitoryController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        private void SetupGrid()
        {
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("currentCount", "배정 인원");
            grid.Columns.Add("maxCount", "최대 인원");
            grid.AddTextActionColumns();
            grid.ActionClicked += OnRowAction;
        }

        private void RenderPage(int page)
        {
            pagination1.TotalCount = _all.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_all.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = _all.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var a in _pageItems)
                grid.Rows.Add(a.dormitoryRoomName, a.currentCount, a.maxCount);
            grid.ResumeLayout();
        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _pageItems.Count) return;
            var target = _pageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = DormMaxCntEditModal.Show(this.FindForm(), target);
                if (edited == null) return;
                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminDormitoryController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    // TODO: API 수정 — await new AdminStudentController().UpdateStudent(target.studentId, edited);
                    var res = await _adminDormitoryController.UpdateDormAssignMaxCnt(edited);
                    if (res?.Status == 200)
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0) _all[idx] = edited;
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
                catch (ApiException ex)
                {
                    MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminDormitoryController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }

            }
        }
        private async void BtnSearch_Click(object? sender, EventArgs e)
        {
            await LoadAndRender(1);
        }
        private async Task LoadCmb()
        {
            var response = await _adminDormitoryController.GetDormRoomAssignStatus();
            if (response?.Status == 200)
            {
                // 출석 전체조회 응답 데이터를 활용하여 콤보박스 목록을 구성
                // 별도 API 호출 없이 LINQ로 중복 제거 후 추출
                // 학생 콤보박스
                var room = response.Data
                    .Select(x => new { x.dormitoryId, x.dormitoryRoomName})
                    .DistinctBy(x => x.dormitoryId)
                    .ToList();
                room.Insert(0, new { dormitoryId = "", dormitoryRoomName = "전체" });
                cmbDormRoom.DataSource = room;
                cmbDormRoom.DisplayMember = "dormitoryRoomName";
                cmbDormRoom.ValueMember = "dormitoryId";
            }
        }
    }
}
