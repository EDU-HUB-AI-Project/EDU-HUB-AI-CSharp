using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using NPOI.HSSF.Util;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.View
{
    public partial class DormitoryView : UserControl
    {
        private AppDataGrid _assignGrid;
        private AppDataGrid _waitingGrid;
        private AppDataGrid _dormInGrid;
        private AppDataGrid _dormOutGrid;

        private Pagination _pagination1;
        private Pagination _pagination2;
        private Pagination _pagination3;
        private Pagination _pagination4;

        private List<DormAssignDto> _assignData = new();
        private List<DormAssignDto> _assignPageItems = new();

        private List<DormInOutDto> _waitingData = new();
        private List<DormInOutDto> _waitingPageItems = new();

        private List<DormInOutDto> _dormInData = new();
        private List<DormInOutDto> _dormInPageItems = new();

        private List<DormInOutDto> _dormOutData = new();

        private readonly AdminDormitoryController _adminDormitoryController = new();
        public DormitoryView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;
            bodyPanel.BackColor = ThemeColors.Background;
            _assignGrid = CreateAssignGrid();
            _waitingGrid = CreateWaitingGrid();
            _dormInGrid = CreateDormInGrid();
            _dormOutGrid = CreateDormOutGrid();
            _pagination1 = new Pagination();
            _pagination2 = new Pagination();
            _pagination3 = new Pagination();
            _pagination4 = new Pagination();
            pageHeader1.SyncClicked += (_, _) => LoadDormView();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDormView();
        //    FixDockOrder();
        }

        //private void FixDockOrder()
        //{
        //    bodyPanel.Controls.SetChildIndex(grid, 0);
        //    bodyPanel.Controls.SetChildIndex(actionPanel, 1);
        //    bodyPanel.Controls.SetChildIndex(pagination1, 2);
        //}

        private async void LoadDormView(bool showOverlay = true)
        {
            bodyPanel.Controls.Clear();
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            try
            {
                _adminDormitoryController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                // ================== 배정 현황 ======================
                var res1 = await _adminDormitoryController.GetDormAssign();
                if (res1?.Status != 200) return;
                
                foreach(var item in res1?.Data)
                {
                    Debug.WriteLine(item);
                }
                _assignData = res1?.Data;
                var panel1 = CreateGridPanel("생활관 배정현황", 20, 30, 1300, 438);
                _assignGrid.Dock = DockStyle.Fill;
                _pagination1.Dock = DockStyle.Bottom;
                _pagination1.PageChanged += (_, page) => RenderAssign(page);
                panel1.Controls.Add(_assignGrid);
                panel1.Controls.Add(_pagination1);
                bodyPanel.Controls.Add(panel1);
                RenderAssign(1);

                // ================== 대기 현황 ======================
                var panel2 = CreateGridPanel("생활관 대기 현황", 20, 500, 400, 300);
                var res2 = await _adminDormitoryController.GetDormWaiting();
                foreach (var item in res2?.Data)
                {
                    Debug.WriteLine(item);
                }
                _waitingData = res2?.Data;
                _waitingGrid.Dock = DockStyle.Fill;
                _pagination2.Dock = DockStyle.Bottom;
                _pagination2.PageChanged += (_, page) => RenderWaiting(page);
                panel2.Controls.Add(_waitingGrid);
                panel2.Controls.Add(_pagination2);
                bodyPanel.Controls.Add(panel2);
                RenderWaiting(1);

                // ================== 입실 현황 ======================
                var panel3 = CreateGridPanel("생활관 입실 현황", 570, 500, 400, 300);
                var res3 = await _adminDormitoryController.GetDormIn();
                foreach(var item in res3?.Data)
                {
                    Debug.WriteLine(item);
                }
                _dormInData = res3?.Data;
                _dormInGrid.Dock = DockStyle.Fill;
                _pagination3.Dock = DockStyle.Bottom;
                _pagination3.PageChanged += (_, page) => RenderDormIn(page);
                panel3.Controls.Add(_dormInGrid);
                panel3.Controls.Add(_pagination3);
                bodyPanel.Controls.Add(panel3);
                RenderDormIn(1);

                // ================== 퇴실 현황 ======================
                var panel4 = CreateGridPanel("생활관 퇴실 현황", 1000, 500, 300, 300);
                var res4 = await _adminDormitoryController.GetDormOut();
                foreach (var item in res4?.Data)
                {
                    Debug.WriteLine(item);
                }
                _dormOutData = res4?.Data;
                _dormOutGrid.AddTextActionColumns(false, false);
                _dormOutGrid.Dock = DockStyle.Fill;
                _pagination4.Dock = DockStyle.Bottom;
                _pagination4.PageChanged += (_, page) => RenderDormOut(page);
                panel4.Controls.Add(_dormOutGrid);
                panel4.Controls.Add(_pagination4);
                bodyPanel.Controls.Add(panel4);
                RenderDormOut(1);
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

        private AppDataGrid CreateAssignGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("eduId", "교육Id");
            grid.Columns.Add("phone", "전화번호");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("assignStatus", "배정상태");
            grid.AddTextActionColumns(true, false);
            grid.ActionClicked += OnRowActionAssign;
            return grid;
        }

        private AppDataGrid CreateWaitingGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.AddTextActionColumns(true, false);
            grid.ActionClicked += OnRowActionWaiting;
            return grid;
        }

        private AppDataGrid CreateDormInGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("dorm", "입실상태");
            grid.AddTextActionColumns(true, false);
            grid.ActionClicked += OnRowActionDormOut;
            return grid;
        }

        private AppDataGrid CreateDormOutGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("dorm", "입실상태");
            grid.AddTextActionColumns(false, false);
            return grid;
        }

        private void RenderAssign(int page)
        {
            _pagination1.TotalCount = _assignData.Count;
            var size = _pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_assignData.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            _pagination1.PageIndex = page;

            _assignPageItems = _assignData.Skip((page - 1) * size).Take(size).ToList();

            _assignGrid.SuspendLayout();
            _assignGrid.Rows.Clear();
            foreach (var a in _assignPageItems)
                _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
            _assignGrid.ResumeLayout();
        }

        private void RenderWaiting(int page)
        {
            _pagination2.TotalCount = _waitingData.Count;
            var size = _pagination2.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_waitingData.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            _pagination2.PageIndex = page;

            _waitingPageItems = _waitingData.Skip((page - 1) * size).Take(size).ToList();

            _waitingGrid.SuspendLayout();
            _waitingGrid.Rows.Clear();
            foreach (var a in _waitingPageItems)
                _waitingGrid.Rows.Add(a.studentName, a.dormitoryRoomName);
            _waitingGrid.ResumeLayout();
        }

        private void RenderDormIn(int page)
        {
            _pagination3.TotalCount = _dormInData.Count;
            var size = _pagination3.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_dormInData.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            _pagination3.PageIndex = page;

            _dormInPageItems = _dormInData.Skip((page - 1) * size).Take(size).ToList();

            _dormInGrid.SuspendLayout();
            _dormInGrid.Rows.Clear();
            foreach (var d in _dormInPageItems)
                _dormInGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.dorm);
            _dormInGrid.ResumeLayout();
        }

        private void RenderDormOut(int page)
        {
            _pagination4.TotalCount = _dormOutData.Count;
            var size = _pagination4.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(_dormOutData.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            _pagination4.PageIndex = page;

            var pageItems = _dormOutData.Skip((page - 1) * size).Take(size).ToList();

            _dormOutGrid.SuspendLayout();
            _dormOutGrid.Rows.Clear();
            foreach (var d in pageItems)
                _dormOutGrid.Rows.Add(d.studentName, d.dorm);
            _dormOutGrid.ResumeLayout();
        }

        private async void OnRowActionAssign(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _assignPageItems.Count) return;
            var target = _assignPageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                var edited = DormAssignModal.Show(this.FindForm(), target);
                if (edited == null) return;
                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminDormitoryController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    // TODO: API 수정 — await new AdminStudentController().UpdateStudent(target.studentId, edited);
                    var res = await _adminDormitoryController.UpdateDormId(target.studentId, edited);
                    if (res?.Status == 200)
                    {
                        var idx = _assignData.IndexOf(target);
                        if (idx >= 0) _assignData[idx] = edited;
                        RenderAssign(_pagination1.PageIndex);
                        LoadDormView(showOverlay: false);
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

        private async void OnRowActionWaiting(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _waitingPageItems.Count) return;
            var target = _waitingPageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                if (MessageBox.Show($"{target.studentName} 학생을 입실 처리하시겠습니까?", "입실 확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "처리 중...");
                _adminDormitoryController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    var dormitoryDto = new DormitoryDto
                    {
                        dormitoryId = target.dormitoryId
                    };
                    var res = await _adminDormitoryController.UpdateDormCurrentCnt(target.studentId, dormitoryDto);
                    if (res?.Status == 200)
                    {
                        // 삭제되면 입실 Grid에서 숨기기(DB 삭제x)
                        _waitingData.Remove(target);
                        RenderWaiting(_pagination2.PageIndex);
                        LoadDormView(showOverlay: false);
                    }
                    else
                    {
                        MessageBox.Show(res?.Message ?? "처리에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private async void OnRowActionDormOut(object? sender, TableActionEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _dormInPageItems.Count) return;
            var target = _dormInPageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                if (MessageBox.Show($"{target.studentName} 학생을 퇴실 처리하시겠습니까?", "퇴실 확인",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

                var overlay = LoadingOverlay.Create(bodyPanel, "처리 중...");
                _adminDormitoryController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    var dormitoryDto = new DormitoryDto
                    {
                        dormitoryId = target.dormitoryId
                    };
                    var res = await _adminDormitoryController.UpdateDormCurrentCntDown(target.studentId, dormitoryDto);
                    if (res?.Status == 200)
                    {
                        // 삭제되면 입실 Grid에서 숨기기(DB 삭제x)
                        _dormInData.Remove(target);
                        RenderDormIn(_pagination3.PageIndex);
                        LoadDormView(showOverlay: false);
                    }
                    else
                    {
                        MessageBox.Show(res?.Message ?? "처리에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private Panel CreateGridPanel(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                Padding = new Padding(10, 35, 10, 10)
            };

            panel.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(226, 232, 240), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };

            panel.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("맑은 고딕", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(12, 10),
                AutoSize = true
            });

            return panel;
        }
    }
}
