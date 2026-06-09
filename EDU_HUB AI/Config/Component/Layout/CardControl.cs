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

        private string _linkText = "";

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

        public string LinkText
        {
            get => _linkText;
            set
            {
                _linkText = value;
                Cursor = string.IsNullOrEmpty(value) ? Cursors.Default : Cursors.Hand;
                Invalidate();
            }
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
            const int margin = 16;

            using var titleFont = new Font("맑은 고딕", 9f);
            using var valueFont = new Font("맑은 고딕", 26f, FontStyle.Bold);
            using var subFont = new Font("맑은 고딕", 8f);
            using var linkFont = new Font("맑은 고딕", 8f, FontStyle.Underline);
            using var gray = new SolidBrush(Color.Gray);
            using var dark = new SolidBrush(Color.FromArgb(15, 23, 42));
            using var blue = new SolidBrush(Color.SteelBlue);
            using var bgBrush = new SolidBrush(Color.FromArgb(220, 220, 220));
            using var fgBrush = new SolidBrush(_accentColor);

            float subY = 38 + g.MeasureString(_value, valueFont).Height + 4;
            float subH = g.MeasureString("A", subFont).Height;
            int barY = Height - 20;
            int barW = Width - margin * 2;

            // 타이틀
            g.DrawString(_title, titleFont, gray, margin, 16);
            // 큰 숫자
            g.DrawString(_value, valueFont, dark, margin, 38);
            // 서브텍스트
            g.DrawString(_subText, subFont, gray, margin, subY);

            // 바로가기 링크
            if (!string.IsNullOrEmpty(_linkText))
            {
                SizeF linkSize = g.MeasureString(_linkText, linkFont);
                float linkY = Math.Min(subY + subH + 4, barY - linkSize.Height - 2);
                g.DrawString(_linkText, linkFont, blue, Width - linkSize.Width - margin, linkY);
            }

            // 하단 프로그레스바
            g.FillRectangle(bgBrush, new Rectangle(margin, barY, barW, 6));
            g.FillRectangle(fgBrush, new Rectangle(margin, barY, barW / 2, 6));
        }
    }
}
