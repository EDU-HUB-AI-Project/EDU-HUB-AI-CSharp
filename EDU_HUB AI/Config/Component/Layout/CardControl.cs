using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.Config.Component.Layout
{
    public partial class CardControl : UserControl
    {
        private string _title = "";
        private string _value = "";
        private string _subText = "";
        private Color _accentColor = Color.Green;

        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }
        public string Value
        {
            get => _value;
            set { _value = value; Invalidate(); }
        }
        public string SubText
        {
            get => _subText;
            set { _subText = value; Invalidate(); }
        }
        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public CardControl()
        {
            InitializeComponent();
            BackColor = Color.White;
            Size = new Size(300, 140);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // 타이틀
            g.DrawString(_title, new Font("맑은 고딕", 9f), new SolidBrush(Color.Gray), 16, 16);
            // 큰 숫자
            g.DrawString(_value, new Font("맑은 고딕", 26f, FontStyle.Bold), new SolidBrush(Color.FromArgb(15, 23, 42)), 16, 38);
            // 서브텍스트
            g.DrawString(_subText, new Font("맑은 고딕", 8f), new SolidBrush(Color.Gray), 16, 90);
            // 하단 프로그레스바
            var barRect = new Rectangle(16, 115, Width - 32, 6);
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 220, 220)), barRect);
            g.FillRectangle(new SolidBrush(_accentColor), new Rectangle(16, 115, (Width - 32) / 2, 6));
        }
    }
}
