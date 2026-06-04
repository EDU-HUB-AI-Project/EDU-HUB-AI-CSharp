using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace EDU_HUB_AI.View
{
    public partial class testForm : Form
    {
        private readonly AdminAttendaceController _adminAttendaceController = new();
        public testForm()
        {
            InitializeComponent();
            btnTest.Click += BtnTest_Click;
            btnTestPost.Click += BtnTestPost_Click;
            btnTestPatch.Click += BtnTestPatch_Click;
            btnTestDelete.Click += BtnTestDelete_Click;
        }


        public async void BtnTest_Click(object? sender, EventArgs e)
        {
            string? studentId = null;
            string? eduId = null;
            string? attendDate = null;
            string? status = null;

            var result = await _adminAttendaceController.GetAttend(studentId, eduId, attendDate, status);

            if (result?.Status == "success")
            {
                foreach (var item in result?.Data)
                {
                    Debug.WriteLine(item);
                }
                MessageBox.Show(result.ToString());
            }
            else
            {
                MessageBox.Show(result?.Message);
            }
        }

        public async void BtnTestPost_Click(object? sender, EventArgs e)
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

        public async void BtnTestPatch_Click(object? sender, EventArgs e)
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

        public async void BtnTestDelete_Click(object? sender, EventArgs e)
        {
            string attendanceId = "ATT_test3";
            await _adminAttendaceController.DeleteAttend(attendanceId);
        }
    }
}
