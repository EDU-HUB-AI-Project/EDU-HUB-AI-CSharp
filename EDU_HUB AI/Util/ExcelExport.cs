using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace EDU_HUB_AI.Util
{
    public class ExcelExport
    {
        public void ExcelExporter(DataTable dt, string filePath)
        {   
            // 엑셀 파일 생성
            IWorkbook wb = new XSSFWorkbook(); //.xlsx(excel 2007 이상)
            //IWorkbook wb = new HSSFWorkbook(); //.xls(excel 2003 이하)

            // 엑셀 워크시트 생성
            ISheet sheet = wb.CreateSheet("Sheet1");

            //헤더 작성
            var headerRow = sheet.CreateRow(0);
            for (int i = 0; i < dt.Columns.Count - 2; i++)
            {
                headerRow.CreateCell(i).SetCellValue(dt.Columns[i].ColumnName);
            }
            // 데이터 작성
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(dt.Rows[i][j]?.ToString());
                }
            }
            //NPOI는 경로를 직접 받아주는 메소드가 없음 따라서 fileStream사용
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                wb.Write(file);
            }
            MessageBox.Show("파일이 저장되었습니다");
        }
    }
}
