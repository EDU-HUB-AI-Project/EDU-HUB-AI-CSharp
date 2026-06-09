using EDU_HUB_AI.Config.Component.Common;
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
    public partial class MainShellForm : Form
    {
        public MainShellForm()
        {
            InitializeComponent();
            navigation1.MenuSelected += (_, key) => NavigateTo(key);
            NavigateTo(MenuKey.Dashboard); // 초기 화면 설정, 추후 대시보드로 변경
        }

        private void NavigateTo(MenuKey key, string? filter = null)
        {
            UserControl? view = key switch
            {
                MenuKey.Trainees => new StudentView(),
                MenuKey.Facilities => new FacilityLocationView(),
                MenuKey.Cafeteria => new CafeteriaView(),
                MenuKey.Transport => new TransportView(),
                MenuKey.Dashboard => new DashboardView(NavigateTo),
                MenuKey.Dormitory => new DormitoryView(),
                MenuKey.Attendance => new AttendanceView(),
                MenuKey.EduInfo => new EduInfoView(),
                MenuKey.Classroom => new ClassroomView(),
                MenuKey.Subject => new SubjectView(),
                _ => null
                // 여기에 본인 MenuKey 에 따른 UserControl 추가
            };

            if(view == null)
            {
                return;
            }
            if(filter != null)
            {
                if(view is AttendanceView av) av.SetFilter(filter);
                if (view is EduInfoView ev) ev.SetFilter(filter);
            }

            contentPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            navigation1.ActiveMenu = key;
        }
    }
}
