using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using System.Diagnostics;


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
        private List<DormInOutDto> _dormOutPageItems = new();

        private Action<DormAssignDto> _assignRow;
        private Action<DormInOutDto> _waitingRow;
        private Action<DormInOutDto> _dormInRow;
        private Action<DormInOutDto> _dormOutRow;

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
            
            _assignRow = a => _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
            _waitingRow = d => _waitingGrid.Rows.Add(d.studentName, d.dormitoryRoomName);
            _dormInRow = d => _dormInGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.dorm);
            _dormOutRow = d => _dormOutGrid.Rows.Add(d.studentName, d.dorm);
            pageHeader1.SyncClicked += (_, _) => LoadDormView();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDormView();
        }

        // Refactoring 방향
        // 기존에는 동일한 로직의 Grid생성과, Render로직이 반복
        // 중복부분을 헬퍼 메소드를 통하여 분리
        // BuildGridPanel : 패널/그리드/페이지네이션 UI 구성
        // RenderGrid<T>  : 페이지 계산 및 데이터 바인딩
        // 각 Grid별로 다른 부분(Rows.Add 컬럼 구성)은 Action<T>를 사용하여 외부에서 주입
        // OnRowAction 계열은 호출 API와 처리 로직이 달라 리팩토링 대상에서 제외
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

                foreach (var item in res1?.Data)
                {
                    Debug.WriteLine(item);
                }
                _assignData = res1?.Data;
                var panel1 = BuildGridPanel("생활관 배정현황", 20, 30, 1300, 438, _assignGrid, _pagination1,
                                  a => _assignPageItems = RenderGrid(_assignData, _pagination1, 1, _assignGrid, _assignRow) // 페이지 이동시 사용
                                  );
                // 최초실행시 필요
                _assignPageItems = RenderGrid(_assignData, _pagination1, 1, _assignGrid, _assignRow);

                // ================== 대기 현황 ======================
                var res2 = await _adminDormitoryController.GetDormWaiting();
                foreach (var item in res2?.Data)
                {
                    Debug.WriteLine(item);
                }
                _waitingData = res2?.Data;
                var panel2 = BuildGridPanel("생활관 대기 현황", 20, 500, 400, 300, _waitingGrid, _pagination2,
                                            d => _waitingPageItems = RenderGrid(_waitingData, _pagination2, 1, _waitingGrid, _waitingRow)
                                            );
                // 최초실행시 필요
                _waitingPageItems = RenderGrid(_waitingData, _pagination2, 1, _waitingGrid, _waitingRow);

                // ================== 입실 현황 ======================
                //var panel3 = CreateGridPanel("생활관 입실 현황", 570, 500, 400, 300);
                var res3 = await _adminDormitoryController.GetDormIn();
                foreach (var item in res3?.Data)
                {
                    Debug.WriteLine(item);
                }
                _dormInData = res3?.Data;
                var panel3 = BuildGridPanel("생활관 입실 현황", 570, 500, 400, 300, _dormInGrid, _pagination3,
                                            d => _dormInPageItems = RenderGrid(_dormInData, _pagination3, 1, _dormInGrid, _dormInRow));
                _dormInPageItems = RenderGrid(_dormInData, _pagination3, 1, _dormInGrid, _dormInRow);

                // ================== 퇴실 현황 ======================
                var res4 = await _adminDormitoryController.GetDormOut();
                foreach (var item in res4?.Data)
                {
                    Debug.WriteLine(item);
                }
                _dormOutData = res4?.Data;
                var panel4 = BuildGridPanel("생활관 퇴실 현황", 1000, 500, 300, 300, _dormOutGrid, _pagination4,
                                            d => _dormOutPageItems = RenderGrid(_dormOutData, _pagination4, 1, _dormOutGrid, _dormOutRow)
                                            );
                _dormOutPageItems = RenderGrid(_dormOutData, _pagination4, 1, _dormOutGrid, _dormOutRow);
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

        // Delegate 활용하여 리펙토링(Action)
        // grid 생성 후 RenderGrid에서 데이터 넣기
        private Panel BuildGridPanel(string title, int x, int y, int w, int h, 
                                    AppDataGrid grid, Pagination pagenation, Action<int> pageChange)
        {
            var panel = CreateGridPanel(title, x, y, w, h);
            grid.Dock = DockStyle.Fill;
            pagenation.Dock = DockStyle.Bottom;
            pagenation.PageChanged += (_, page) => pageChange(page);
            panel.Controls.Add(pagenation);
            panel.Controls.Add(grid);
            bodyPanel.Controls.Add(panel);
            return panel;
        }

        private List<T> RenderGrid<T>(List<T> data, Pagination pagination, int page, AppDataGrid grid, Action<T> addRow)
        {
            pagination.TotalCount = data.Count;
            var size = pagination.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(data.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination.PageIndex = page;

            var pageItems = data.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var item in pageItems)
                addRow(item);
            grid.ResumeLayout();

            return pageItems;
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

        
        // 생활관 배정 현황 RowAction
        private async void OnRowActionAssign(object? sender, TableActionEventArgs e)
        {
            Action<DormAssignDto> addAssignRow = a => _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
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

        // 생활관 대기 현황 RowAction
        private async void OnRowActionWaiting(object? sender, TableActionEventArgs e)
        {
            Action<DormInOutDto> waitingRow = d => _waitingGrid.Rows.Add(d.studentName, d.dormitoryRoomName);
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
        // 생활관 입실 현황 RowAction
        private async void OnRowActionDormOut(object? sender, TableActionEventArgs e)
        {
            Action<DormInOutDto> dormInRow = d => _dormInGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.dorm);
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



        //private void RenderAssign(int page)
        //{
        //    _pagination1.TotalCount = _assignData.Count;
        //    var size = _pagination1.PageSize;
        //    var totalPages = Math.Max(1, (int)Math.Ceiling(_assignData.Count / (double)size));
        //    page = Math.Clamp(page, 1, totalPages);
        //    _pagination1.PageIndex = page;

        //    _assignPageItems = _assignData.Skip((page - 1) * size).Take(size).ToList();

        //    _assignGrid.SuspendLayout();
        //    _assignGrid.Rows.Clear();
        //    foreach (var a in _assignPageItems)
        //        _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
        //    _assignGrid.ResumeLayout();
        //}

        //private void RenderWaiting(int page)
        //{
        //    _pagination2.TotalCount = _waitingData.Count;
        //    var size = _pagination2.PageSize;
        //    var totalPages = Math.Max(1, (int)Math.Ceiling(_waitingData.Count / (double)size));
        //    page = Math.Clamp(page, 1, totalPages);
        //    _pagination2.PageIndex = page;

        //    _waitingPageItems = _waitingData.Skip((page - 1) * size).Take(size).ToList();

        //    _waitingGrid.SuspendLayout();
        //    _waitingGrid.Rows.Clear();
        //    foreach (var a in _waitingPageItems)
        //        _waitingGrid.Rows.Add(a.studentName, a.dormitoryRoomName);
        //    _waitingGrid.ResumeLayout();
        //}

        //private void RenderDormIn(int page)
        //{
        //    _pagination3.TotalCount = _dormInData.Count;
        //    var size = _pagination3.PageSize;
        //    var totalPages = Math.Max(1, (int)Math.Ceiling(_dormInData.Count / (double)size));
        //    page = Math.Clamp(page, 1, totalPages);
        //    _pagination3.PageIndex = page;

        //    _dormInPageItems = _dormInData.Skip((page - 1) * size).Take(size).ToList();

        //    _dormInGrid.SuspendLayout();
        //    _dormInGrid.Rows.Clear();
        //    foreach (var d in _dormInPageItems)
        //        _dormInGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.dorm);
        //    _dormInGrid.ResumeLayout();
        //}

        //private void RenderDormOut(int page)
        //{
        //    _pagination4.TotalCount = _dormOutData.Count;
        //    var size = _pagination4.PageSize;
        //    var totalPages = Math.Max(1, (int)Math.Ceiling(_dormOutData.Count / (double)size));
        //    page = Math.Clamp(page, 1, totalPages);
        //    _pagination4.PageIndex = page;

        //    var pageItems = _dormOutData.Skip((page - 1) * size).Take(size).ToList();

        //    _dormOutGrid.SuspendLayout();
        //    _dormOutGrid.Rows.Clear();
        //    foreach (var d in pageItems)
        //        _dormOutGrid.Rows.Add(d.studentName, d.dorm);
        //    _dormOutGrid.ResumeLayout();
        //}
    }
}
