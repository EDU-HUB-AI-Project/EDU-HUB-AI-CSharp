using EDU_HUB_AI.Config.Component.Basic;
using EDU_HUB_AI.Config.Component.Layout;
using EDU_HUB_AI.Config.Theme;
using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.Config.Component.Domain
{
    public partial class DormMaxCntEditModal : AppModal
    {
        private readonly DormitoryDto? _source;
        private readonly TextField _txtDormitoryID;
        private readonly TextField _txtRoomMaxCnt;
        private readonly TextField _txtModifyMax;
        private readonly AdminDormitoryController _adminDormitoryController = new();

        public DormitoryDto? Result { get; private set; }
        public DormMaxCntEditModal(DormitoryDto? source)
        {
            _source = source;
            ModalTitle = "최대 인원 변경";
            ConfirmText = "저장";

            var stack = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                Padding = new Padding(0),
                BackColor = ThemeColors.Surface
            };

            stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _txtDormitoryID = AddField(stack, "생활관 id", source?.dormitoryId, "", 0);
            _txtDormitoryID.ReadOnly = true;
            _txtRoomMaxCnt = AddField(stack, "최대 인원", source?.maxCount.ToString() ?? "미정", "", 1);
            Body.Controls.Add(stack);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        public static DormitoryDto? Show(IWin32Window owner, DormitoryDto? source)
        {
            using var modal = new DormMaxCntEditModal(source);
            return modal.ShowDialog(owner) == DialogResult.OK ? modal.Result : null;
        }

        protected override void OnConfirm()
        {
            var maxCnt = _txtModifyMax.Text.Trim();

            if (maxCnt == null || maxCnt == "")
            {
                MessageBox.Show("변경하려는 최대 인원을 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(!int.TryParse(maxCnt, out int maxCount))
            {
                MessageBox.Show("숫자만 입력해주세요", "입력오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Result = _source != null ? CopyOf(_source) : new DormitoryDto();
            Result.maxCount= Convert.ToInt32(maxCnt);
            base.OnConfirm();
        }

        private static TextField AddField(TableLayoutPanel parent, string label, string? value, string placeholder, int row)
        {
            parent.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var field = new TextField
            {
                FieldLabel = label,
                Text = value ?? "",
                Placeholder = placeholder,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 14)
            };
            parent.Controls.Add(field, 0, row);
            return field;
        }

        private static DormitoryDto CopyOf(DormitoryDto d) => new()
        {
            dormitoryId = d.dormitoryId,
            eduId = d.eduId,
            currentCount = d.currentCount,
            maxCount = d.maxCount,
            delYn = d.delYn,
            dormitoryRoomName = d.dormitoryRoomName
        };
    }
}
