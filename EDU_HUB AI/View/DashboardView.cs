using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WinForms;
using SkiaSharp;

namespace EDU_HUB_AI.View
{
    public partial class DashboardView : UserControl
    {   
        public DashboardView()
        {
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
        private async void LoadDashboard()
        {
            // =================== 데이터 불러오기 ===================

            var res1 = await new AdminDashBoardController().GetAttendCount();
            var res2 = await new AdminDashBoardController().GetPopularFeature();
            var res3 = await new AdminDashBoardController().GetDormStats();
            var res4 = await new AdminDashBoardController().GetPrintCountByHour();
            if (res1?.Status != 200 || res2?.Status != 200 ||
                res3?.Status != 200 || res4?.Status != 200) return;

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

            bodyPanel.Controls.Clear();

            // ========================== 출석 현황 ===================================

            bodyPanel.Controls.Add(new CardControl { 
                                    Title = "출석 현황", 
                                    Value = $"{attend}명", 
                                    SubText = GetRatio(attend), 
                                    AccentColor = Color.Green, 
                                    Location = new Point(20, 20) 
                                });
            bodyPanel.Controls.Add(new CardControl { 
                                    Title = "결석 현황", 
                                    Value = $"{absence}명", 
                                    SubText = GetRatio(absence), 
                                    AccentColor = Color.FromArgb(220, 53, 69), 
                                    Location = new Point(340, 20) 
                                });
            bodyPanel.Controls.Add(new CardControl { 
                                    Title = "지각 현황", 
                                    Value = $"{late}명", 
                                    SubText = GetRatio(late), 
                                    AccentColor = Color.DarkOrange, 
                                    Location = new Point(660, 20) 
                                });
            bodyPanel.Controls.Add(new CardControl { 
                                Title = "조퇴 현황", 
                                Value = $"{earlyLeave}명", 
                                SubText = GetRatio(earlyLeave), 
                                AccentColor = Color.DarkGoldenrod, 
                                Location = new Point(980, 20) 
                                });

            var barPanel = CreateChartPanel("출석 현황 분포", 20, 200, 1260, 130);
            barPanel.Controls.Add(new CartesianChart
            {
                Dock = DockStyle.Fill,
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
            });
            bodyPanel.Controls.Add(barPanel);

            // ========================== 출석 현황 끝===================================
            // --------------------------------------------------------------------------
            // ========================== 인기 기능 =====================================

            var piePanel = CreateChartPanel("인기 기능", 20, 345, 400, 330);
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

            var dormPanel = CreateChartPanel("생활관 입실 현황", 440, 345, 300, 330);
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

            var printPanel = CreateChartPanel("명찰 발급 시간대", 760, 345, 520, 330);
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
        }


        // 차트 panel
        private Panel CreateChartPanel(string title, int x, int y, int width, int height)
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
