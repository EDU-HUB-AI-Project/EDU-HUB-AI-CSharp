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
using SkiaSharp;

namespace EDU_HUB_AI.View
{
    public partial class DashboardView : UserControl
    {
        private AdminDashBoardController _adminDashBoardController = new AdminDashBoardController();

        private readonly Action<MenuKey, string?>? _navigateTo;

        public DashboardView(Action<MenuKey, string?>? navigateTo = null)
        {
            _navigateTo = navigateTo;
            InitializeComponent();
            BackColor = ThemeColors.Background;

            bodyPanel.BackColor = ThemeColors.Background;

            pageHeader1.SyncClicked += (_, _) => LoadDashboard();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadDashboard();
        }

        // ===== 데이터 연동 지점 (여기만 바꾸면 됨) =====
        private async void LoadDashboard(bool showOverlay = true)
        {
            var overlay = showOverlay ? LoadingOverlay.Create(bodyPanel, "데이터 로딩중...") : null;
            _adminDashBoardController.OnRetry = (attempt, max) => overlay?.UpdateMessage($"서버 연결 중...\n재시도 {attempt}/{max}");
            try
            {
                // =================== 데이터 불러오기 ===================

                var res1 = await _adminDashBoardController.GetAttendCount();
                var res2 = await _adminDashBoardController.GetPopularFeature();
                var res3 = await _adminDashBoardController.GetDormStats();
                var res4 = await _adminDashBoardController.GetPrintCountByHour();
                var res5 = await _adminDashBoardController.GetLogTop10();
                var res6 = await _adminDashBoardController.GetEduStats();
                if (res1?.Status != 200 || res2?.Status != 200 ||
                    res3?.Status != 200 || res4?.Status != 200 || res5?.Status != 200 || res6?.Status != 200) return;

                // =============  출석현황에 사용할 데이터 ===============

                int total = (int)res1?.Data["TOTAL_ATTENDANCE"].GetInt32();
                int attend = (int)res1?.Data["ATTEND"].GetInt32();
                int absence = (int)res1?.Data["ABSENCE"].GetInt32();
                int late = (int)res1?.Data["LATE"].GetInt32();
                int earlyLeave = (int)res1?.Data["EARLY_LEAVE"].GetInt32();

                // =============== 입실 현황에 사용할 데이터 ===============

                int totalStudents = (int)res3?.Data["totalStudents"].GetInt32();
                int assignCount = (int)res3?.Data["assignCount"].GetInt32();
                int unassigned = totalStudents - assignCount;

                // =============== 명찰 발급 시간대 데이터 =================
                var hourData = new double[12];
                foreach (var item in res4?.Data)
                {
                    int hour = (int)item["PRINT_HOUR"].GetInt32();
                    if (hour >= 7 && hour <= 18)
                        hourData[hour - 7] = (int)item["PRINTING_COUNT"].GetInt32();
                }

                string GetRatio(int value) =>
                    total > 0 ? $"{value} / {total} ({(double)value / total * 100:F2}%)" : "0 / 0 (0%)";

                // ================= 교육 과정 관련 데이터 ===================
                int activeEduCount = res6.Data["ACTIVE_EDU_COUNT"].GetInt32();
                int totalStudentsEdu = res6.Data["TOTAL_STUDENTS"].GetInt32();
                double avgAttendRate = res6.Data["AVG_ATTEND_RATE"].GetDouble();
                int warningEduCount = res6.Data["WARNING_EDU_COUNT"].GetInt32();

                bodyPanel.Controls.Clear();

                int W = bodyPanel.ClientSize.Width;
                int gap = 20;

                // ========================== 출석 현황 ===================================
                int cardW = (W - 3 * gap) / 4;

                var cardAttend = new CardControl
                {
                    Title = "출석 현황",
                    Value = $"{attend}명",
                    SubText = GetRatio(attend),
                    AccentColor = ThemeColors.Ok,
                    ProgressValue = total > 0 ? (float)attend / total : 0f,
                    Size = new Size(cardW, 165),
                    Location = new Point(0, 0),
                    LinkText = "바로가기 →"
                };

                var cardAbsence = new CardControl
                {
                    Title = "결석 현황",
                    Value = $"{absence}명",
                    SubText = GetRatio(absence),
                    AccentColor = ThemeColors.Danger,
                    ProgressValue = total > 0 ? (float)absence / total : 0f,
                    Size = new Size(cardW, 165),
                    Location = new Point(cardW + gap, 0),
                    LinkText = "바로가기 →"
                };

                var cardLate = new CardControl
                {
                    Title = "지각 현황",
                    Value = $"{late}명",
                    SubText = GetRatio(late),
                    AccentColor = ThemeColors.Warn,
                    ProgressValue = total > 0 ? (float)late / total : 0f,
                    Size = new Size(cardW, 165),
                    Location = new Point(2 * (cardW + gap), 0),
                    LinkText = "바로가기 →"
                };

                var cardEarlyLeave = new CardControl
                {
                    Title = "조퇴 현황",
                    Value = $"{earlyLeave}명",
                    SubText = GetRatio(earlyLeave),
                    AccentColor = ThemeColors.Warn,
                    ProgressValue = total > 0 ? (float)earlyLeave / total : 0f,
                    Size = new Size(cardW, 165),
                    Location = new Point(3 * (cardW + gap), 0),
                    LinkText = "바로가기 →"
                };

                cardAttend.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "출석");
                cardAbsence.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "결석");
                cardLate.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "지각");
                cardEarlyLeave.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "조퇴");

                bodyPanel.Controls.Add(cardAttend);
                bodyPanel.Controls.Add(cardAbsence);
                bodyPanel.Controls.Add(cardLate);
                bodyPanel.Controls.Add(cardEarlyLeave);

                // ========================== 출석 현황 끝 =================================
                // -------------------------------------------------------------------------
                // ========================== 출석 현황 분포 ================================
                int barY = 165 + gap;
                var barPanel = CreateChartPanel("출석 현황 분포", 0, barY, W, 180);
                
                var btnAttend = new AppButton
                {
                    Location = new Point(W - 160, 6),
                    Size = new Size(150, 10),
                    Text = "출석현황"
                };
                var adminChart = new CartesianChart
                {
                    Location = new Point(30, 50),
                    Size = new Size(W - 60, 105),
                    Series = new ISeries[]
                    {
                    new StackedRowSeries<double> {
                        Name = "조퇴",
                        Values = new double[] { earlyLeave },
                        Fill = new SolidColorPaint(new SKColor(255, 193, 7))
                    },
                    new StackedRowSeries<double> {
                        Name = "지각",
                        Values = new double[] { late },
                        Fill = new SolidColorPaint(SKColors.DarkOrange)
                    },
                    new StackedRowSeries<double> {
                        Name = "결석",
                        Values = new double[] { absence },
                        Fill = new SolidColorPaint(new SKColor(220, 53, 69))
                    },
                    new StackedRowSeries<double> {
                        Name = "출석",
                        Values = new double[] { attend },
                        Fill = new SolidColorPaint(SKColors.Green) }
                        },
                    LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom,
                    XAxes = new[] {
                            new Axis
                            {
                                IsVisible = false
                            }
                        },
                    YAxes = new[] {
                            new Axis
                            {
                                IsVisible = false
                            }
                        }
                };
                barPanel.Controls.Add(adminChart);
                barPanel.Controls.Add(btnAttend);
                bodyPanel.Controls.Add(barPanel);

                // ========================== 출석 현황 분포 끝===================================
                // --------------------------------------------------------------------------
                // ========================== 인기 기능 =====================================

                int midY = barY + 180 + gap;
                int pieW = (int)((W - 2 * gap) * 0.30);
                int dormW = (int)((W - 2 * gap) * 0.22);
                int printW = W - pieW - dormW - 2 * gap;

                var piePanel = CreateChartPanel("인기 기능", 0, midY, pieW, 330);
                piePanel.Controls.Add(new PieChart
                {
                    Dock = DockStyle.Fill,
                    Series = res2.Data.Select(item => new PieSeries<double>
                    {
                        Name = item["ACTION"].GetString(),
                        Values = new double[] { (int)item["CNT"].GetInt32() }
                    }).ToArray(),
                    LegendPosition = LiveChartsCore.Measure.LegendPosition.Right
                });
                bodyPanel.Controls.Add(piePanel);

                // ========================== 인기 기능 끝 ==================================
                // --------------------------------------------------------------------------
                // ======================== 생활관 입실 현황 ================================

                var dormPanel = CreateChartPanel("생활관 입실 현황", pieW + gap, midY, dormW, 330);
                dormPanel.Controls.Add(new PieChart
                {
                    Dock = DockStyle.Fill,
                    Series = new ISeries[]
                    {
                     new PieSeries<double> {
                         Name = "배정",
                         Values = new double[] { assignCount },
                         Fill = new SolidColorPaint(SKColors.SteelBlue),
                         InnerRadius = 60
                     },
                     new PieSeries<double> {
                         Name = "미배정",
                         Values = new double[] { unassigned },
                         Fill = new SolidColorPaint(new SKColor(226, 232, 240)),
                         InnerRadius = 60
                     }
                    },
                    LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom
                });
                bodyPanel.Controls.Add(dormPanel);

                // ========================== 인기 기능 끝 ==================================
                // --------------------------------------------------------------------------
                // ======================== 명찰 발급 트래픽 ================================

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
                    XAxes = new[] {
                    new Axis {
                        Labels = Enumerable.Range(7, 12).Select(h => $"{h}시").ToArray()
                    }
                }
                });
                bodyPanel.Controls.Add(printPanel);

                // ======================== 명찰 발급 트래픽 끝 =============================
                // --------------------------------------------------------------------------
                // ======================== 최근 키오스크 연동 ==============================

                int botY = midY + 330 + gap;
                int logW = pieW;
                int remaining = W - logW - gap;
                int eduCardW = (remaining - gap) / 2;
                int eduRow2Y = botY + 165 + gap;

                var dgvLogTop10 = new AppDataGrid();
                dgvLogTop10.Dock = DockStyle.Fill;
                dgvLogTop10.Columns.Add("createdAt", "시간");
                dgvLogTop10.Columns.Add("action", "활동내역");
                dgvLogTop10.Width = 380;
                var logGrid = CreateChartPanel("최근 키오스크 연동 이벤트", 0, botY, logW, 330);
                dgvLogTop10.Rows.Clear();
                foreach (var item in res5?.Data)
                {
                    string time = DateTime.Parse(item["createdAt"].GetString()).ToString("yy-MM-dd HH:mm");
                    string action = item["action"].GetString();
                    dgvLogTop10.Rows.Add(time, action);
                }
                logGrid.Controls.Add(dgvLogTop10);
                bodyPanel.Controls.Add(logGrid);
                // ====================== 최근 키오스크 연동 끝 =============================
                // --------------------------------------------------------------------------
                // ========================== 교육 과정 연동 ================================

                var cardActiveEdu = new CardControl
                {
                    Title = "진행중 과정",
                    Value = $"{activeEduCount}개",
                    SubText = "현재 운영중인 교육과정",
                    AccentColor = ThemeColors.Primary,
                    Size = new Size(eduCardW, 165),
                    Location = new Point(logW + gap, botY),
                    LinkText = "바로가기 →"
                };

                var cardTotalStudents = new CardControl
                {
                    Title = "총 수강생",
                    Value = $"{totalStudentsEdu}명",
                    SubText = "진행중 과정 수강생 합계",
                    AccentColor = ThemeColors.Ok,
                    Size = new Size(eduCardW, 165),
                    Location = new Point(logW + gap + eduCardW + gap, botY),
                    LinkText = "바로가기 →"
                };

                var cardAvgAttend = new CardControl
                {
                    Title = "평균 출석률",
                    Value = $"{avgAttendRate:F2}%",
                    SubText = "오늘 기준 전체 출석률",
                    AccentColor = ThemeColors.Warn,
                    ProgressValue = (float)(avgAttendRate / 100),
                    Size = new Size(eduCardW, 165),
                    Location = new Point(logW + gap, eduRow2Y),
                    LinkText = "바로가기 →"
                };

                var cardWarning = new CardControl
                {
                    Title = "주의 필요 과정",
                    Value = $"{warningEduCount}개",
                    SubText = "출석률 80% 미만 과정",
                    AccentColor = ThemeColors.Danger,
                    Size = new Size(eduCardW, 165),
                    Location = new Point(logW + gap + eduCardW + gap, eduRow2Y),
                    LinkText = "바로가기 →"
                };

                cardActiveEdu.Click += (_, _) => _navigateTo?.Invoke(MenuKey.EduInfo, "ACTIVE");
                cardTotalStudents.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Trainees, null);
                cardAvgAttend.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "TODAY");
                cardWarning.Click += (_, _) => _navigateTo?.Invoke(MenuKey.Attendance, "TODAY");


                bodyPanel.Controls.Add(cardActiveEdu);
                bodyPanel.Controls.Add(cardTotalStudents);
                bodyPanel.Controls.Add(cardAvgAttend);
                bodyPanel.Controls.Add(cardWarning);

                // ======================== 교육 과정 연동 끝 ===============================
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
                _adminDashBoardController.OnRetry = null;
                overlay?.Close();
                overlay?.Dispose();
            }
        }


        // 차트 panel
        private Panel CreateChartPanel(string title, int x, int y, int width, int height)
            {
                var panel = new Panel
                {
                    Location = new Point(x, y),
                    Size = new Size(width, height),
                    BackColor = ThemeColors.Surface,
                    Padding = new Padding(10, 35, 10, 10)
                };

                panel.Paint += (s, e) =>
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
    }
}
