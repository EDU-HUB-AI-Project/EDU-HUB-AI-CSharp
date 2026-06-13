using EDU_HUB_AI.Config.Component.Basic;
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
        private AppDataGrid _dormInOutGrid;

        private Pagination _pagination1;
        private Pagination _pagination2;
        private Pagination _pagination3;

        private List<DormAssignDto> _assignData = new();
        private List<DormAssignDto> _assignPageItems = new();

        private List<DormInOutDto> _waitingData = new();
        private List<DormInOutDto> _waitingPageItems = new();

        private List<DormInOutDto> _dormInOutData = new();
        private List<DormInOutDto> _dormInOutPageItems = new();

        private Action<DormAssignDto> _assignRow;
        private Action<DormInOutDto> _waitingRow;
        private Action<DormInOutDto> _dormInOutRow;

        private readonly AdminDormitoryController _adminDormitoryController = new();
        private bool _isLoading = false; // LoadDormView 중복 호출 방지 플래그 
        public DormitoryView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;
            bodyPanel.BackColor = ThemeColors.Background;

            _assignGrid = CreateAssignGrid();
            _waitingGrid = CreateWaitingGrid();
            _dormInOutGrid = CreateDormInOutGrid();

            _pagination1 = new Pagination();
            _pagination2 = new Pagination();
            _pagination3 = new Pagination();

            _assignRow = a => _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
            _waitingRow = d => _waitingGrid.Rows.Add(d.studentName, d.dormitoryRoomName);
            _dormInOutRow = d => _dormInOutGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.checkIn, d.checkOut);
            pageHeader1.SyncClicked += (_, _) => LoadDormView();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            BeginInvoke(() => LoadDormView()); // 너비 계산 시점 문제로 UI가 완전히 로딩된 후 LoadDormView 실행
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
            if (IsDisposed || !IsHandleCreated) return; // View가 이미 소멸된 경우 무시
            if (_isLoading) return; // View가 이미 소멸된 경우 무시
            _isLoading = true;
            ClearBodyPanel();

            // BuildGridPanel()에서 Panel의 자식으로 추가되는 컨트롤은
            // ClearBodyPanel()이 Panel을 Dispose할 때 자식도 함께 Dispose된다.
            // 따라서 매 호출마다 새로 생성해야 한다.
            _pagination1 = new Pagination();
            _pagination2 = new Pagination();
            _pagination3 = new Pagination();

            _assignGrid = CreateAssignGrid();      
            _waitingGrid = CreateWaitingGrid(); 
            _dormInOutGrid = CreateDormInOutGrid();

            // 위에서 Grid를 새로 생성했으므로 Action도 새 Grid를 참조하도록 재정의
            _assignRow = a => _assignGrid.Rows.Add(a.studentName, a.eduId, a.phone, a.dormitoryRoomName, a.assignStatus);
            _waitingRow = d => _waitingGrid.Rows.Add(d.studentName, d.dormitoryRoomName);
            _dormInOutRow = d => _dormInOutGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.checkIn, d.checkOut);

            // 위치 배정
            // 여백 직접 정의
            int pad = 20;                                        
            int W = bodyPanel.ClientSize.Width - (pad * 2);     
            int gap = 20;
            int assignH = 550;
            // 상단 여백 + 배정그리드 높이 + gap
            int bottomY = pad + assignH + gap;
            // 왼쪽의 30%만
            int waitingW = (int)(W * 0.30);                     
            int inOutW = W - waitingW - gap;
            int bottomH = 400;

            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            try
            {
                _adminDormitoryController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                // ================== 배정 현황 ======================
                var res1 = await _adminDormitoryController.GetDormAssign();
                if (IsDisposed || !IsHandleCreated) return; // await 복귀 시점에 View가 소멸됐을 수 있음
                if (res1?.Status != 200) return;
                if (res1?.Status != 200) return;
                _assignData = res1.Data;
                var panel1 = BuildGridPanel("생활관 배정현황",
                   pad, pad, W, assignH,                   
                    _assignGrid, _pagination1,
                    a => _assignPageItems = RenderGrid(_assignData, _pagination1, a, _assignGrid, _assignRow));
                _assignPageItems = RenderGrid(_assignData, _pagination1, 1, _assignGrid, _assignRow);

                // ================== 대기 현황 ======================
                var res2 = await _adminDormitoryController.GetDormWaiting();
                if (IsDisposed || !IsHandleCreated) return; // await 복귀 시점에 View가 소멸됐을 수 있음
                _waitingData = res2.Data;
                var panel2 = BuildGridPanel("생활관 대기 현황",
                    pad, bottomY, waitingW, bottomH,      
                    _waitingGrid, _pagination2,
                    d => _waitingPageItems = RenderGrid(_waitingData, _pagination2, d, _waitingGrid, _waitingRow));
                _waitingPageItems = RenderGrid(_waitingData, _pagination2, 1, _waitingGrid, _waitingRow);

                // ================== 입/퇴실 현황 ======================
                var res3 = await _adminDormitoryController.GetDormInOut();
                if (IsDisposed || !IsHandleCreated) return; // await 복귀 시점에 View가 소멸됐을 수 있음
                _dormInOutData = res3.Data;
                var panel3 = BuildGridPanel("생활관 입/퇴실 현황",
                   pad + waitingW + gap, bottomY, inOutW, bottomH,  
                    _dormInOutGrid, _pagination3,
                    d => _dormInOutPageItems = RenderGrid(_dormInOutData, _pagination3, d, _dormInOutGrid, _dormInOutRow));
                _dormInOutPageItems = RenderGrid(_dormInOutData, _pagination3, 1, _dormInOutGrid, _dormInOutRow);
            }
            catch (ApiException ex)
            {
                if (IsDisposed) return;
                MessageBox.Show(ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                if (IsDisposed) return;
                MessageBox.Show($"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminDormitoryController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
                _isLoading = false; // 로딩 완료, 다음 호출 허용
            }
        }
        

        // Delegate 활용하여 리펙토링(Action)
        // grid 생성 후 RenderGrid에서 데이터 넣기
        private Panel BuildGridPanel(string title, int x, int y, int w, int h, 
                                    AppDataGrid grid, Pagination pagination, Action<int> pageChange)
        {
            var panel = CreateGridPanel(title, x, y, w, h);
            grid.Dock = DockStyle.Fill;
            pagination.Dock = DockStyle.Bottom;
            pagination.PageChanged += (_, page) => pageChange(page);
            pagination.Height = 38;
            panel.Controls.Add(pagination);
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
            grid.Columns.Add("phone", "연락처");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("assignStatus", "배정상태");
            grid.AddTextActionColumns(true, false);
            // 생성된 칼럼의 텍스트만 변경
            if (grid.Columns[AppDataGrid.EditColumnName] is DataGridViewLinkColumn editCol)
            {
                editCol.Text = "배정";
            }
            grid.ActionClicked += OnRowActionAssign;
            grid.CellFormatting += OnCellFormatting;
            return grid;
        }

        private AppDataGrid CreateWaitingGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.AddTextActionColumns(true, false);
            // 생성된 칼럼의 텍스트만 변경
            if (grid.Columns[AppDataGrid.EditColumnName] is DataGridViewLinkColumn editCol)
            {
                editCol.Text = "입실";
            }
            grid.ActionClicked += OnRowActionWaiting;
            grid.CellFormatting += OnCellFormatting;
            return grid;
        }

        private AppDataGrid CreateDormInOutGrid()
        {
            var grid = new AppDataGrid();
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("dormitoryRoomName", "호실");
            grid.Columns.Add("checkIn", "입실");
            grid.Columns.Add("checkOut", "퇴실");
            grid.AddTextActionColumns(true, false);
            grid.ActionClicked += OnRowActionDormOut;
            grid.CellFormatting += OnCellFormatting;

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
                if (!ConfirmModal.Show(this.FindForm(), "입실 확인", $"{target.studentName} 학생을 입실 처리하시겠습니까?", "입실", ButtonVariant.Primary))
                    return;

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
            Action<DormInOutDto> dormInRow = d => _dormInOutGrid.Rows.Add(d.studentName, d.dormitoryRoomName, d.dorm);
            if (e.RowIndex < 0 || e.RowIndex >= _dormInOutPageItems.Count) return;
            var target = _dormInOutPageItems[e.RowIndex];

            if (e.Action == TableAction.Edit)
            {
                if (!ConfirmModal.Show(this.FindForm(), "퇴실 확인", $"{target.studentName} 학생을 퇴실 처리하시겠습니까?", "퇴실", ButtonVariant.Danger))
                    return;

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

       private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (sender is not AppDataGrid grid) return;
            if (grid.Columns[e.ColumnIndex].Name == "dormitoryRoomName" || grid.Columns[e.ColumnIndex].Name == "phone")
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private Panel CreateGridPanel(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                Padding = new Padding(10, 35, 10, 0)
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
        private void ClearBodyPanel()
        {
            var controls = bodyPanel.Controls.OfType<Control>().ToList();
            bodyPanel.Controls.Clear();
            foreach (var c in controls)
                c.Dispose();
        }
    }
}
