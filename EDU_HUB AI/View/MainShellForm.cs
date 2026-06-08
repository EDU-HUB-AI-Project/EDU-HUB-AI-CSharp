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
            NavigateTo(MenuKey.Trainees); // 초기 화면 설정, 추후 대시보드로 변경
        }

        private void NavigateTo(MenuKey key)
        {
            UserControl? view = key switch
            {
                MenuKey.Trainees => new StudentView(),
                MenuKey.Facilities => new CafeteriaView(),
                //MenuKey.Attendance => new AttendanceView(),
                MenuKey.Test => new TestView(),
                _ => null
                // 여기에 본인 MenuKey 에 따른 UserControl 추가
            };

            if(view == null)
            {
                return;
            }

            contentPanel.Controls.Clear();
            view.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(view);
            navigation1.ActiveMenu = key;
        }
    }
}
