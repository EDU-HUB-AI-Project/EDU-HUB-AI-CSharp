using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Common;
using EDU_HUB_AI.Config.Component.Data;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using NPOI.SS.Formula.Functions;
using SkiaSharp;

namespace EDU_HUB_AI.View
{
    public partial class DashboardView : UserControl
    {
        private readonly AdminDashBoardController _controller = new();
        private readonly Action<MenuKey, string?>? _navigateTo;

        public DashboardView(Action<MenuKey, string?>? navigateTo = null)
        {
            _navigateTo = navigateTo;
            InitializeComponent();
            pageHeader1.SyncClicked += (_, _) => LoadDashboard();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDashboard();
        }

        private async void LoadDashboard(bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            _controller.OnRetry = (attempt, max) =>
                overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
            try
            {
                var res1 = await _controller.GetAttendCount();
                var res2 = await _controller.GetPopularFeature();
                var res3 = await _controller.GetDormStats();
                var res4 = await _controller.GetPrintCountByHour();
                var res5 = await _controller.GetLogTop10();
                var res6 = await _controller.GetEduStats();

                if (res1?.Status != 200 || res2?.Status != 200 ||
                    res3?.Status != 200 || res4?.Status != 200 ||
                    res5?.Status != 200 || res6?.Status != 200) return;

                // ── 데이터 파싱 ────────────────────────────────
                int total = res1.Data["TOTAL_ATTENDANCE"].GetInt32();
                int attend = res1.Data["ATTEND"].GetInt32();
                int absence = res1.Data["ABSENCE"].GetInt32();
                int late = res1.Data["LATE"].GetInt32();
                int earlyLeave = res1.Data["EARLY_LEAVE"].GetInt32();

                int totalStudents = res3.Data["totalStudents"].GetInt32();
                int assignCount = res3.Data["assignCount"].GetInt32();
                int unassigned = totalStudents - assignCount;

                var hourData = new double[12];
                foreach (var item in res4.Data)
                {
                    int hour = item["PRINT_HOUR"].GetInt32();
                    if (hour >= 7 && hour <= 18)
                        hourData[hour - 7] = item["PRINTING_COUNT"].GetInt32();
                }

                int activeEduCount = res6.Data["ACTIVE_EDU_COUNT"].GetInt32();
                int totalStudentsEdu = res6.Data["TOTAL_STUDENTS"].GetInt32();
                double avgAttendRate = res6.Data["AVG_ATTEND_RATE"].GetDouble();
                int warningEduCount = res6.Data["WARNING_EDU_COUNT"].GetInt32();

                string GetRatio(int v) =>
                    total > 0 ? $"{v} / {total} ({(double)v / total * 100:F2}%)" : "0 / 0 (0%)";

                // ── 기존 컨트롤 정리 ──────────────────────────
                ClearBodyPanel();

                int W = bodyPanel.ClientSize.Width;
                int gap = 20;

                // ── 출석 카드 4개 ──────────────────────────────
                int cardW = (W - 3 * gap) / 4;

                var cardAttend = MakeCard("출석 현황", $"{attend}명", GetRatio(attend),
                    ThemeColors.Ok, total > 0 ? (float)attend / total : 0f,
                    new Point(0, 0), cardW);
                var cardAbsence = MakeCard("결석 현황", $"{absence}명", GetRatio(absence),
                    ThemeColors.Danger, total > 0 ? (float)absence / total : 0f,
                    new Point(cardW + gap, 0), cardW);
                var cardLate = MakeCard("지각 현황", $"{late}명", GetRatio(late),
                    ThemeColors.Warn, total > 0 ? (float)late / total : 0f,
                    new Point(2 * (cardW + gap), 0), cardW);
                var cardEarlyLeave = MakeCard("조퇴 현황", $"{earlyLeave}명", GetRatio(earlyLeave),
                    ThemeColors.DashBoardEarlyLeave, total > 0 ? (float)earlyLeave / total : 0f,
                    new Point(3 * (cardW + gap), 0), cardW);

                cardAttend.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "출석");
                cardAbsence.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "결석");
                cardLate.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "지각");
                cardEarlyLeave.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "조퇴");

                bodyPanel.Controls.Add(cardAttend);
                bodyPanel.Controls.Add(cardAbsence);
                bodyPanel.Controls.Add(cardLate);
                bodyPanel.Controls.Add(cardEarlyLeave);

                // ── 출석 분포 차트 ─────────────────────────────
                int barY = 165 + gap;
                var barPanel = CreateChartPanel("출석 현황 분포", 0, barY, W, 180);
                barPanel.Controls.Add(new CartesianChart
                {
                    Location = new Point(30, 50),
                    Size = new Size(W - 60, 105),
                    TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,
                    Series = new ISeries[]
                    {
                        new StackedRowSeries<double>
                        {
                            Name = $"출석 ({attend}명)",
                            Values = new double[] { attend },
                            Fill = new SolidColorPaint(new SKColor(16, 185, 129)),
                            IsHoverable = false,
                            Stroke = null
                        },
                        new StackedRowSeries<double>
                        {
                            Name = $"결석 ({absence}명)",
                            Values = new double[] { absence },
                            Fill = new SolidColorPaint(new SKColor(239, 68, 68)),
                            IsHoverable = false,
                            Stroke = null
                        },
                        new StackedRowSeries<double>
                        {
                            Name = $"지각 ({late}명)",
                            Values = new double[] { late },
                            Fill = new SolidColorPaint(new SKColor(245, 158, 11)),
                            IsHoverable = false,
                            Stroke = null
                        },
                        new StackedRowSeries<double>
                        {
                            Name = $"조퇴 ({earlyLeave}명)",
                            Values = new double[] { earlyLeave },
                            Fill = new SolidColorPaint(new SKColor(255, 193, 7)),
                            IsHoverable = false,
                            Stroke = null
                        }
                    },
                LegendPosition = LiveChartsCore.Measure.LegendPosition.Right,
                    XAxes = new[] { new Axis { IsVisible = false, MinLimit = 0, MaxLimit = total } },
                YAxes = new[] { new Axis { IsVisible = false } }
                });
                bodyPanel.Controls.Add(barPanel);

                // ── 인기 기능 / 생활관 / 명찰 시간대 ─────────────
                int midY = barY + 180 + gap;
                int pieW = (int)((W - 2 * gap) * 0.30);
                int dormW = (int)((W - 2 * gap) * 0.22);
                int printW = W - pieW - dormW - 2 * gap;

                var piePanel = CreateChartPanel("인기 기능", 0, midY, pieW, 330);
                piePanel.Controls.Add(new PieChart
                {
                    Dock = DockStyle.Fill,
                    TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,
                    Series = res2.Data
                             .Where(item => item["ACTION"].GetString() != "교육생 안내")
                            .Select(item => 
                            {
                                string action = item["ACTION"].GetString();
                                int cnt = item["CNT"].GetInt32();
                                return new PieSeries<double>
                            {
                                Name = $"{action} ({cnt}건)",
                                Values = new double[] { cnt },
                                IsHoverable = false
                                };
                }).ToArray(),
                    LegendPosition = LiveChartsCore.Measure.LegendPosition.Right
                });
                bodyPanel.Controls.Add(piePanel);

                var dormPanel = CreateChartPanel("생활관 입실 현황", pieW + gap, midY, dormW, 330);
                dormPanel.Controls.Add(new PieChart
                {
                    Dock = DockStyle.Fill,
                    TooltipPosition = LiveChartsCore.Measure.TooltipPosition.Hidden,
                    Series = new ISeries[]
                    {
                        new PieSeries<double> { Name = $"배정 ({assignCount}명)",
                            Values = new double[] { assignCount },
                            Fill = new SolidColorPaint(SKColors.SteelBlue),
                            IsHoverable = false,
                            InnerRadius = 60 },
                        new PieSeries<double> { Name = $"미배정 ({unassigned}명)",
                            Values = new double[] { unassigned },
                            Fill = new SolidColorPaint(new SKColor(226, 232, 240)),
                            IsHoverable = false,
                            InnerRadius = 60 }
                    },
                    LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
                });
                bodyPanel.Controls.Add(dormPanel);

                var printPanel = CreateChartPanel("명찰 발급 시간대", pieW + gap + dormW + gap, midY, printW, 330);
                printPanel.Controls.Add(new CartesianChart
                {
                    Dock = DockStyle.Fill,
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Name = "명찰 발급",
                            Values = hourData,
                            Fill = null
                        }
                    },
                    XAxes = new[] { new Axis
                    {
                        Labels = Enumerable.Range(7, 12).Select(h => $"{h}시").ToArray()
                    }}
                });
                bodyPanel.Controls.Add(printPanel);

                // ── 최근 이벤트 로그 ───────────────────────────
                int botY = midY + 330 + gap;
                int logW = pieW;

                var dgvLog = new AppDataGrid
                {
                    Dock = DockStyle.Fill,
                    EnableHeadersVisualStyles = false,
                    ReadOnly = true,
                    RowHeadersVisible = false,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BorderStyle = BorderStyle.None,
                    BackgroundColor = ThemeColors.Surface,
                    GridColor = ThemeColors.TableBorder,
                    ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single,
                    ColumnHeadersHeight = 38,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                    ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = ThemeColors.TableHeader,
                        ForeColor = ThemeColors.Text,
                        Font = ThemeFonts.TableHeader,
                        SelectionBackColor = ThemeColors.TableHeader,
                        Alignment = DataGridViewContentAlignment.MiddleLeft,
                        Padding = new Padding(12, 0, 8, 0)
                    },
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = ThemeColors.Surface,
                        ForeColor = ThemeColors.Text,
                        Font = ThemeFonts.TableCell,
                        SelectionBackColor = ThemeColors.TableHover,
                        SelectionForeColor = ThemeColors.Text,
                        Padding = new Padding(12, 0, 8, 0),
                        WrapMode = DataGridViewTriState.False
                    },
                    AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                    {
                        BackColor = ThemeColors.TableStripe
                    }
                };
                dgvLog.RowTemplate.Height = 44;
                dgvLog.Columns.Add("createdAt", "시간");
                dgvLog.Columns.Add("action", "활동내역");

                foreach (var item in res5.Data)
                {
                    string time = DateTime.Parse(item["createdAt"].GetString()).ToString("yy-MM-dd HH:mm");
                    string action = item["action"].GetString();
                    dgvLog.Rows.Add(time, action);
                }

                var logGrid = CreateChartPanel("최근 키오스크 연동 이벤트", 0, botY, logW, 330);
                logGrid.Controls.Add(dgvLog);
                bodyPanel.Controls.Add(logGrid);

                // ── 교육과정 KPI 카드 4개 ──────────────────────
                int remaining = W - logW - gap;
                int eduCardW = (remaining - gap) / 2;
                int eduRow2Y = botY + 165 + gap;

                var cardActiveEdu = MakeCard("진행중 과정", $"{activeEduCount}개",
                    "현재 운영중인 교육과정", ThemeColors.Primary, 0f,
                    new Point(logW + gap, botY), eduCardW);
                var cardTotalStudentsEdu = MakeCard("총 수강생", $"{totalStudentsEdu}명",
                    "진행중 과정 수강생 합계", ThemeColors.Ok, 0f,
                    new Point(logW + gap + eduCardW + gap, botY), eduCardW);
                var cardAvgAttend = MakeCard("평균 출석률", $"{avgAttendRate:F2}%",
                    "오늘 기준 전체 출석률", ThemeColors.Warn,
                    (float)(avgAttendRate / 100),
                    new Point(logW + gap, eduRow2Y), eduCardW);
                var cardWarning = MakeCard("주의 필요 과정", $"{warningEduCount}개",
                    "출석률 80% 미만 과정", ThemeColors.Danger, 0f,
                    new Point(logW + gap + eduCardW + gap, eduRow2Y), eduCardW);

                cardActiveEdu.Click += (_, _) => _navigateTo?.Invoke(MenuKey.EduInfo, "ACTIVE");
                cardTotalStudentsEdu.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Trainees, null);
                cardAvgAttend.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "TODAY");
                cardWarning.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "TODAY");

                bodyPanel.Controls.Add(cardActiveEdu);
                bodyPanel.Controls.Add(cardTotalStudentsEdu);
                bodyPanel.Controls.Add(cardAvgAttend);
                bodyPanel.Controls.Add(cardWarning);
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
                _controller.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }

        // ── 헬퍼 ─────────────────────────────────────────────
        private static CardControl MakeCard(string title, string value, string sub,
            Color accent, float progress, Point location, int width) => new()
            {
                Title = title,
                Value = value,
                SubText = sub,
                AccentColor = accent,
                ProgressValue = progress,
                LinkText = "바로가기 →",
                Size = new Size(width, 165),
                Location = location
            };

        private Panel CreateChartPanel(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = ThemeColors.Surface,
                Padding = new Padding(10, 35, 10, 10)
            };
            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(ThemeColors.Border, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
            };
            panel.Controls.Add(new Label
            {
                Text = title,
                Font = ThemeFonts.Section,
                ForeColor = ThemeColors.Text,
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