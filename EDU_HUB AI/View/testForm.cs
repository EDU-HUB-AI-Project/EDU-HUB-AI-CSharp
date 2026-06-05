using EDU_HUB_AI.Controller;
using EDU_HUB_AI.exception;
using EDU_HUB_AI.Model;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace EDU_HUB_AI.View
{
    public partial class testForm : Form
    {
        private readonly AdminAttendaceController _adminAttendaceController = new();
        private readonly AdminKioskLogController _adminKioskLogController = new();
        public testForm()
        {
            InitializeComponent();
            btnTest.Click += BtnTest_Click;
            btnTestPost.Click += BtnTestPost_Click;
            btnTestPatch.Click += BtnTestPatch_Click;
            btnTestDelete.Click += BtnTestDelete_Click;
            
            btnKLGet.Click += BtnKLGet_Click;
            btnKLPost.Click += BtnKLPost_Click;
            btnKLPatch.Click += BtnKLPatch_Click;
            btnKLDel.Click += BtnKLDel_Click;
        }

        private void BtnKLPatch_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public async void BtnTest_Click(object? sender, EventArgs e)
        {
            string? studentId = null;
            string? eduId = null;
            string? attendDate = null;
            string? status = null;

            try
            {
                var result = await _adminAttendaceController.GetAttend(studentId, eduId, attendDate, status);
                Debug.WriteLine(result?.Status);
                foreach (var item in result?.Data)
                {
                    Debug.WriteLine(item);
                }
                MessageBox.Show(result.ToString());
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnTestPost_Click(object? sender, EventArgs e)
        {
            try
            {
                AttendDto attendDto = new AttendDto
                {
                    attendanceId = "ATT_test3",
                    studentId = "STU_26007",
                    attendDate = "2026-06-04",
                    status = "조퇴",
                    message = "병원진료"
                };
                var result = await _adminAttendaceController.InsertAttend(attendDto);
                Debug.WriteLine(result);
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnTestPatch_Click(object? sender, EventArgs e)
        {
            try
            {
                string studentId = "STU_26007";
                AttendDto attendDto = new AttendDto
                {
                    status = "지각",
                    message = "행정 업무",
                    attendDate = "2026-06-04"
                };
                var result = await _adminAttendaceController.UpdateAttendMsg(studentId, attendDto);
                Debug.WriteLine(result);
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnTestDelete_Click(object? sender, EventArgs e)
        {
            try
            {
                string attendanceId = "ATT_test3";
                await _adminAttendaceController.DeleteAttend(attendanceId);
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnKLGet_Click(object? sender, EventArgs e)
        {
            string? logId = null;
            string? action = null;
            string? createdAt = null;

            try
            {
                var result = await _adminKioskLogController.GetKiostLog(logId, action, createdAt);
                Debug.WriteLine(result?.Status);
                foreach (var item in result?.Data)
                {
                    Debug.WriteLine(item);
                }
                MessageBox.Show(result.ToString());

            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnKLPost_Click(object? sender, EventArgs e)
        {
            KioskLogDto kioskLogDto = new KioskLogDto
            {
                action="구내 식당"
            };
            try
            {
                var result = await _adminKioskLogController.InsertLog(kioskLogDto);
                Debug.WriteLine(result);
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async void BtnKLDel_Click(object? sender, EventArgs e)
        {
            KioskLogDto kioskLogDto = new KioskLogDto
            {
                logId = "52",
                action = "구내 식당",
                createdAt = "2026-06-05"
            };
            try
            {
                var result = await _adminKioskLogController.DeleteLog(kioskLogDto);
                Debug.WriteLine(result);
            }
            catch (ApiException ex)
            {
                MessageBox.Show($"오류: [{ex.Status}] {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
