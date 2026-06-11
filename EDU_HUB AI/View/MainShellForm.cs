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
        private static readonly MenuKey[] _menuShortcuts =
            [
                MenuKey.Dashboard,   // Ctrl+1
                MenuKey.Trainees,    // Ctrl+2
                MenuKey.Attendance,  // Ctrl+3
                MenuKey.Dormitory,   // Ctrl+4
                MenuKey.EduInfo,     // Ctrl+5
                MenuKey.Subject,     // Ctrl+6
                MenuKey.Facilities,  // Ctrl+7
                MenuKey.Cafeteria,   // Ctrl+8
                MenuKey.Transport,   // Ctrl+9
            ];


        public MainShellForm()
        {
            InitializeComponent();
            KeyPreview = true;
            KeyDown += OnShellKeyDown;

            navigation1.MenuSelected += (_, key) => NavigateTo(key);
            NavigateTo(MenuKey.Dashboard);
        }

        private void OnShellKeyDown(object? sender, KeyEventArgs e)
        {
            if(e.Control)
            {
                var digit = e.KeyCode - Keys.D1;
                if (digit >= 0 && digit < _menuShortcuts.Length)
                {
                    NavigateTo(_menuShortcuts[digit]);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }

                if(e.KeyCode == Keys.F)
                {
                    contentPanel.Controls.OfType<ISearchFocusable>().FirstOrDefault()?.FocusSearch();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
                }
            }

            if(e.KeyCode == Keys.F5)
            {
                NavigateTo(navigation1.ActiveMenu);
                e.Handled = true;
            }
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
                MenuKey.DormRoom => new DormRoomView(),
                MenuKey.Attendance => new AttendanceView(),
                MenuKey.EduInfo => new EduInfoView(),
                MenuKey.Classroom => new ClassroomView(),
                MenuKey.Subject => new SubjectView(),
                _ => null
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
