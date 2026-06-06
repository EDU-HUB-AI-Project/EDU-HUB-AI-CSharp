using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;
using System.Text;

namespace EDU_HUB_AI.Util
{
    public class ExcelImport
    {
        public DataTable ExcelImporter(string filePath)
        {
            DataTable dt = new DataTable();
            IWorkbook wb;
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(filePath).ToLower() == ".xls") wb = new HSSFWorkbook(file);
                else wb = new XSSFWorkbook(file); //.xlsx일 경우

                ISheet sheet = wb.GetSheetAt(0);


                var headerRow = sheet.GetRow(0);
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    dt.Columns.Add(headerRow.GetCell(i).ToString());
                }
                // 데이터 읽기
                for (int i = 1; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;
                    DataRow dataRow = dt.NewRow();
                    for (int j = 0; j < row.LastCellNum; j++)
                    {
                        var cell = row.GetCell(j);
                        if (cell == null)
                        {
                            dataRow[j] = null;
                        }
                        // 날짜 셀이면 DateCellValue로 읽기
                        else if (cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                        {
                            dataRow[j] = cell.DateCellValue?.ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            dataRow[j] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dataRow);
                }
                return dt;
            }
        }
    }
}
