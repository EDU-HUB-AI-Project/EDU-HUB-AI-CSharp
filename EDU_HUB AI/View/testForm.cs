using EDU_HUB_AI.Controller;
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
    public partial class testForm : Form
    {
        private readonly AdminAttendaceController adminAttendaceController = new();
        public testForm()
        {
            InitializeComponent();
            btnTest.Click += BtnTest_Click;
        }


        public async void BtnTest_Click(object? sender, EventArgs e)
        {
            var result = await adminAttendaceController.GetAttend();

            if (result == null)
            {
                Console.WriteLine("데이터 없음");
                return;
            }
            else
            {
                MessageBox.Show(result.ToString());
            }
        }
    }
}
