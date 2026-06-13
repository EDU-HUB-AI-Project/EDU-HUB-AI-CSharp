using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Domain;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using System.Data;


namespace EDU_HUB_AI.View
{
    public partial class AttendanceView : UserControl, ISearchFocusable
    {
        private List<AttendDto> _all = new();
        private List<AttendDto> _pageItems = new();
        private List<AttendDto> _filtered = new();

        private DataTable _dtAttend = new DataTable();
        private readonly ExcelExport excelExport = new ExcelExport();
        private readonly ExcelImport excelImport = new ExcelImport();
        private readonly AdminAttendaceController _adminAttendaceController = new AdminAttendaceController();

        private string? _pendingFilter;

        public AttendanceView()
        {
            InitializeComponent();
            BackColor = ThemeColors.Background;

            SetupGrid();
            bodyPanel.BackColor = ThemeColors.Background;
            pagination1.BackColor = ThemeColors.Background;

            tableCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, tableCard.Width - 1, tableCard.Height - 1);
            };
            filterCard.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border);
                e.Graphics.DrawRectangle(pen, 0, 0, filterCard.Width - 1, filterCard.Height - 1);
            };

            pageHeader1.SyncClicked += async (_, _) => await LoadAndRender(1);
            btnCreate.Click += OnCreate;
            pagination1.PageChanged += (_, page) => RenderPage(page);
            btnExport.Click += BtnExport_Click;
            btnImport.Click += BtnImport_Click;

            grid.PageNavigationRequested += (_, nav) =>
            {
                switch (nav)
                {
                    case PageNavigation.Next: pagination1.GoToNext(); break;
                    case PageNavigation.Prev: pagination1.GoToPrev(); break;
                    case PageNavigation.First: pagination1.GoToFirst(); break;
                    case PageNavigation.Last: pagination1.GoToLast(); break;
                }
            };
            // 교육과정 선택 시 해당 시작/종료 일자 적용
            cmbEdu.SelectedIndexChanged += (_, _) =>
            {
                dynamic? selected = cmbEdu.SelectedItem;
                string? startDate = selected?.eduStartDate;
                string? endDate = selected?.eduEndDate;

                if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
                {
                    dtpStartDate.Checked = true;
                    dtpStartDate.Value = DateTime.Parse(startDate);
                    dtpEndDate.Checked = true;
                    dtpEndDate.Value = DateTime.Parse(endDate);
                }
                else
                {
                    dtpStartDate.Checked = false;
                    dtpEndDate.Checked = false;
                }

                ApplySearchFilter();
                RenderPage(1);
            };
            cmbStatus.SelectedIndexChanged += (_, _) => { ApplySearchFilter(); RenderPage(1); };
            dtpStartDate.ValueChanged += (_, _) => { ApplySearchFilter(); RenderPage(1); };
            dtpEndDate.ValueChanged += (_, _) => { ApplySearchFilter(); RenderPage(1); };
            txtSearch.TextChanged += (_, _) => { ApplySearchFilter(); RenderPage(1); };
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            FixDockOrder();
            LoadCmbStatus();
            await LoadCmb();
            ApplyPendingFilter();
            await LoadAndRender(1);
            grid.Focus();
        }

        private void FixDockOrder()
        {
            bodyPanel.Controls.SetChildIndex(tableCard, 0);
            bodyPanel.Controls.SetChildIndex(gapPanel, 1);
            bodyPanel.Controls.SetChildIndex(filterCard, 2);
            bodyPanel.Controls.SetChildIndex(pagination1, 3);
        }

        // ===== 데이터 연동 지점 =====
        private async Task<List<AttendDto>> LoadData()
        {            
            var res = await _adminAttendaceController.GetAttend();
            return res?.Data ?? new List<AttendDto>();
        }

        private async Task LoadAndRender(int page, bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            _adminAttendaceController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
            try
            {
                _all = await LoadData();
                ApplySearchFilter();
                RenderPage(page);
            }
            catch (ApiException ex)
            {
                MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminAttendaceController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ===== 그리드 =====
        private void SetupGrid()
        {
            grid.Columns.Add("studentName", "이름");
            grid.Columns.Add("phone", "연락처");
            grid.Columns.Add("eduName", "교육과정명");
            grid.Columns.Add("attendDate", "해당일자");
            grid.Columns.Add("status", "상태");
            grid.Columns.Add("message", "사유");
            grid.AddTextActionColumns();
            grid.ActionClicked += OnRowAction;
            grid.CellFormatting += OnCellFormatting;
        }

        private void RenderPage(int page)
        {
            bool hasFilter = cmbEdu.SelectedIndex > 0 || cmbStatus.SelectedIndex > 0 || 
                            !string.IsNullOrEmpty(txtSearch.Text.Trim()) || 
                            dtpStartDate.Checked || dtpEndDate.Checked; 
            var source = hasFilter ? _filtered : _all;
            pagination1.TotalCount = source.Count;
            var size = pagination1.PageSize;
            var totalPages = Math.Max(1, (int)Math.Ceiling(source.Count / (double)size));
            page = Math.Clamp(page, 1, totalPages);
            pagination1.PageIndex = page;

            _pageItems = source.Skip((page - 1) * size).Take(size).ToList();

            grid.SuspendLayout();
            grid.Rows.Clear();
            foreach (var a in _pageItems)
            {
                var idx = grid.Rows.Add(a.studentName, a.phone, a.eduName, a.attendDate, a.status, a.message);
                grid.Rows[idx].Tag = a;
            }
            grid.ResumeLayout();

            ConvertToTable(source);
        }

        // ===== CRUD =====
        protected async void OnCreate(object? sender, EventArgs e)
        {
            var created = AttendEditModal.Show(this.FindForm(), null);
            if (created == null) return;

            var overlay = LoadingOverlay.Create(bodyPanel, "등록 중...");
            _adminAttendaceController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중... \n재시도 {attempt}/{max}");

            try
            {
                var res = await _adminAttendaceController.InsertAttend(created);
                if (res?.Status == 200)
                {
                    await LoadAndRender(int.MaxValue, showOverlay: false);
                }
                else
                {
                    MessageBox.Show(this.FindForm(), res?.Message ?? "등록에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (ApiException ex)
            {
                MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다. \n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _adminAttendaceController.OnRetry = null;
                overlay.Close();
                overlay.Dispose();
            }


        }

        private async void OnRowAction(object? sender, TableActionEventArgs e)
        {
            if(e.Action == TableAction.New)
            {
                OnCreate(sender, EventArgs.Empty);
                return;
            }

            var target = e.Tag as AttendDto;
            if(target == null)
            {
                return;
            }

            if (e.Action == TableAction.Edit)
            {
                var edited = AttendEditModal.Show(this.FindForm(), target);
                if (edited == null) return;
                var overlay = LoadingOverlay.Create(bodyPanel, "수정 중...");
                _adminAttendaceController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    var res = await _adminAttendaceController.UpdateAttendMsg(target.studentId, edited);
                    if(res?.Status == 200) 
                    {
                        var idx = _all.IndexOf(target);
                        if (idx >= 0) _all[idx] = edited;
                        RenderPage(pagination1.PageIndex);
                    }
                    else
                    {
                        MessageBox.Show(this.FindForm(), res?.Message ?? "수정에 실패했습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    
                }
                catch (ApiException ex)
                {
                    MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminAttendaceController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }

            }
            else if (e.Action == TableAction.Delete)
            {
                if (!ConfirmModal.Show(this.FindForm(), "삭제 확인", $"'{target.studentName}'을(를) 삭제할까요?"))
                    return;

                var overlay = LoadingOverlay.Create(bodyPanel, "삭제 중...");
                _adminAttendaceController.OnRetry = (attempt, max) => overlay.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
                try
                {
                    await _adminAttendaceController.DeleteAttend(target.attendanceId);
                    _all.Remove(target);
                    _filtered.Remove(target);
                    RenderPage(pagination1.PageIndex);
                }
                catch (ApiException ex)
                {
                    MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this.FindForm(), $"요청 중 오류가 발생했습니다.\n{ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _adminAttendaceController.OnRetry = null;
                    overlay.Close();
                    overlay.Dispose();
                }
            }
        }

        private void ConvertToTable(List<AttendDto> list)
        {
            _dtAttend = new DataTable();
            // dgv의 칼럼이름 사용
            foreach (DataGridViewColumn col in grid.Columns)
            {
                _dtAttend.Columns.Add(col.HeaderText);
            }
            // dt에 row 추가 이때 dgv순서에 맞게 매핑
            foreach (var items in list)
            {
                _dtAttend.Rows.Add(items.studentName, items.eduName, items.attendDate,
                                    items.status, items.message);
            }
        }

        private void LoadCmbStatus()
        {
            cmbStatus.Items.Add("전체");
            cmbStatus.Items.Add("출석");
            cmbStatus.Items.Add("결석");
            cmbStatus.Items.Add("지각");
            cmbStatus.Items.Add("조퇴");
            cmbStatus.SelectedIndex = 0;
        }
        // eduId에 콤보박스 추가
        private async Task LoadCmb()
        {
            var response = await _adminAttendaceController.GetAttend();
            if (response?.Status == 200)
            {
                // 출석 전체조회 응답 데이터를 활용하여 콤보박스 목록을 구성
                // 별도 API 호출 없이 LINQ로 중복 제거 후 추출
                // 교육과정 콤보박스
                var edus = response.Data
                    .Select(x => new { x.eduId, x.eduName, x.eduStartDate, x.eduEndDate })
                    .DistinctBy(x => x.eduId)
                    .ToList();

                edus.Insert(0, new { eduId = "", eduName = "전체", eduStartDate = (string?)null, eduEndDate = (string?)null });
                cmbEdu.DataSource = edus;
                cmbEdu.DisplayMember = "eduName";
                cmbEdu.ValueMember = "eduId";
            }
        }

        // 출석 상태별 배경 색 변경 및 grid 내부 정렬
        private void OnCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e) 
        {
            // ============= 출석상태에 따른 row의 배경 색상 설정 ===============
            if (e.RowIndex < 0) return;
            if (grid.Columns[e.ColumnIndex] is DataGridViewLinkColumn)
            {
                return;
            }

            var statusCol = grid.Columns["status"];
            if (statusCol == null) return;

            var value = grid.Rows[e.RowIndex].Cells[statusCol.Index].Value;
            if (value == null) return;
            var status = value.ToString();
            if (grid.Columns[e.ColumnIndex].Name == "status")
            {
                if (status == "결석")
                {
                    e.CellStyle.ForeColor = ThemeColors.OkText;
                    e.CellStyle.BackColor = ThemeColors.DangerBg;
                    e.CellStyle.SelectionForeColor = ThemeColors.TableSelectedText;
                    e.CellStyle.SelectionBackColor = ThemeColors.TableSelected;
                }
                else if (status == "지각")
                {
                    e.CellStyle.ForeColor = ThemeColors.OkText;
                    e.CellStyle.BackColor = ThemeColors.WarnBg;
                    e.CellStyle.SelectionForeColor = ThemeColors.TableSelectedText;
                    e.CellStyle.SelectionBackColor = ThemeColors.TableSelected;
                }
                else if (status == "조퇴")
                {
                    e.CellStyle.ForeColor = ThemeColors.OkText;
                    e.CellStyle.BackColor = Color.FromArgb(255, 237, 213);
                    e.CellStyle.SelectionForeColor = ThemeColors.TableSelectedText;
                    e.CellStyle.SelectionBackColor = ThemeColors.TableSelected;
                }
            }
            
            if (grid.Columns[e.ColumnIndex].Name == "phone")
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }
        
        // 엑셀로 내보내기
        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                saveFileDialog.DefaultExt = "xlsx";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    excelExport.ExcelExporter(_dtAttend, filePath);
                }
            }
        }
        // 엑셀에서 저장하기
        private async void BtnImport_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                openFileDialog.DefaultExt = "xlsx";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;
                        DataTable dt = excelImport.ExcelImporter(filePath);
                        List<AttendDto> list = new List<AttendDto>();
                        foreach (DataRow row in dt.Rows)
                        {
                            string studentId = row[0]?.ToString();
                            string attendDate = row[1]?.ToString();
                            string status = row[2]?.ToString();
                            string message = row[3]?.ToString();
                            if(string.IsNullOrEmpty(studentId))
                            {
                                throw new Exception($"{dt.Rows.IndexOf(row) + 1}행 : 이름이 비어있습니다.");
                            }
                            if (string.IsNullOrEmpty(attendDate))
                            {
                                MessageBox.Show(this.FindForm(), $"{dt.Rows.IndexOf(row) + 1}행: 출석일자가 비어있습니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (string.IsNullOrEmpty(status))
                            {
                                MessageBox.Show(this.FindForm(), $"{dt.Rows.IndexOf(row) + 1}행: 출석상태가 비어있습니다.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            if (status != "출석" && string.IsNullOrEmpty(message))
                            {
                                MessageBox.Show(this.FindForm(), $"{dt.Rows.IndexOf(row) + 1}행: {status}의 경우 사유를 입력해주세요.", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            list.Add(new AttendDto
                            {
                                studentId = studentId,
                                attendDate = attendDate,
                                status = status,
                                message = message
                            });
                        }
                        var response = await _adminAttendaceController.InsertAttendList(list);
                        if (response?.Status == 200)
                        {
                            LoadAndRender(int.MaxValue);
                            MessageBox.Show(this.FindForm(), "저장되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (ApiException ex)
                    {
                        MessageBox.Show(this.FindForm(), ex.Message, "서버 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this.FindForm(), $"오류가 발생했습니다.\n{ex.Message}");
                    }
                }
            }
        }

        // ============ 필터링 ============
        public void SetFilter(string filter)
        {
            _pendingFilter = filter;
        }

        private void ApplyPendingFilter()
        {
            if(_pendingFilter == null)
            {
                return;
            }

            switch (_pendingFilter)
            {
                case "TODAY":
                    dtpStartDate.Checked = true;
                    dtpStartDate.Value = DateTime.Today;
                    dtpEndDate.Checked = true;
                    dtpEndDate.Value = DateTime.Today;
                    break;
                case "출석":
                case "결석":
                case "지각":
                case "조퇴":
                    dtpStartDate.Checked = true;
                    dtpStartDate.Value = DateTime.Today;
                    dtpEndDate.Checked = true;
                    dtpEndDate.Value = DateTime.Today;
                    cmbStatus.SelectedItem = _pendingFilter;
                    break;
            }

            _pendingFilter = null;
        }

        private void ApplySearchFilter()
        {
            var result = _all.AsEnumerable();

            var eduId = cmbEdu.SelectedValue?.ToString();
            if (!string.IsNullOrEmpty(eduId))
                result = result.Where(a => a.eduId == eduId);
            var status = cmbStatus.SelectedIndex > 0
                ? cmbStatus.SelectedItem?.ToString()
                : null;
            if (!string.IsNullOrEmpty(status))
                result = result.Where(a => a.status == status);
            var startDate = dtpStartDate.Checked ? dtpStartDate.ToDateString() : null;
            if(!string.IsNullOrEmpty(startDate))
                result = result.Where(a => string.Compare(a.attendDate, startDate) >= 0);
            var endDate = dtpEndDate.Checked ? dtpEndDate.ToDateString() : null;
            if (!string.IsNullOrEmpty(endDate))
                result = result.Where(a => string.Compare(a.attendDate, endDate) <= 0);
            var search = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(search))
                result = result.Where(a => a.studentName?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);

            _filtered = result.ToList();
        }

        // ============ ISearchFocusable ============
        public void FocusSearch() => txtSearch.Focus();

        // ============ 키보드 이벤트 ============
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (ActiveControl is TextBox or ComboBox)
                return base.ProcessCmdKey(ref msg, keyData);
            switch (keyData)
            {
                case Keys.Control | Keys.Right: pagination1.GoToNext(); return true;
                case Keys.Control | Keys.Left: pagination1.GoToPrev(); return true;
                case Keys.Control | Keys.Home: pagination1.GoToFirst(); return true;
                case Keys.Control | Keys.End: pagination1.GoToLast(); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
