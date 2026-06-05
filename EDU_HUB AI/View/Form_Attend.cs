using EDU_HUB_AI.Controller;
using EDU_HUB_AI.Model;
using EDU_HUB_AI.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDU_HUB_AI.View
{
    public partial class Form_Attend : Form
    {
        private readonly AdminAttendaceController _adminAttendaceController = new();
        private ApiResponse<List<AttendDto>> attendList = new();
        private readonly ExcelExport excelExport = new ExcelExport();
        private List<AttendDto> _listAttend = new();
        private DataTable _dtAttend = new DataTable();
        public Form_Attend()
        {
            InitializeComponent();
            PageInit();
            btnExport.Click += BtnExport_Click;
        }

        private void BtnBack_Click(object? sender, EventArgs e)
        {
            this.Owner?.Show();
            this.Close();
        }

        private async void PageInit()
        {
            await LoadAttend();
        }

        private async Task LoadAttend()
        {
            string? studentId = null;
            string? eduId = null;
            string? attendDate = null;
            string? status = null;

            var response = await _adminAttendaceController.GetAttend(studentId, eduId, attendDate, status);
            if (response?.Status == 200)
            {
                _listAttend = new();
                foreach (var item in response.Data)
                {
                    _listAttend.Add(item);          
                }
                bsAttend.DataSource = _listAttend;
                dgvAttend.DataSource = bsAttend;
                this.ConvertToTable(_listAttend);
            }
        }
        // ExcelExport를 공통함수로 분리하면서 데이터 구조에 관계없이 처리하기 위해 DataTable을 사용
        private void ConvertToTable (List<AttendDto> list)
        {
            _dtAttend = new DataTable();
            // dgv의 칼럼이름 사용
            foreach (DataGridViewColumn col in dgvAttend.Columns)
            {
                _dtAttend.Columns.Add(col.HeaderText);
            }
            // dt에 row 추가 이때 dgv순서에 맞게 매핑
            foreach(var items in list){
                _dtAttend.Rows.Add(items.attendanceId, items.studentId, items.eduId, items.status,
                            items.message, items.createdAt, items.updatedAt, items.attendDate);
            }
        } 
        // 엑셀로 내보내기 버튼 클릭하면 엑셀 파일에 저장
        private void BtnExport_Click (object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files(*.xlsx) | *.xlsx";
                saveFileDialog.DefaultExt = "xlsx";
                if(saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    excelExport.ExcelExporter(_dtAttend, filePath);
                }
            }
        }
        
    }
}
