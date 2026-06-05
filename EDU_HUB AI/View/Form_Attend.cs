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
    public partial class Form_Attend : Form
    {
        public Form_Attend()
        {
            InitializeComponent();
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            this.Owner?.Show();
            this.Close();
        }
    }
}
