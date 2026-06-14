using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;

namespace EDU_HUB_AI.Util
{
    public class ExcelImport
    {
        public DataTable ExcelImporter<T>(string filePath) where T : new() // 반드시 new로 생성가능한 타입이어야 한다 아래 typeof(T).GetProperties() 때문
        {
            DataTable dt = new DataTable();
            IWorkbook wb;
            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                if (Path.GetExtension(filePath).ToLower() == ".xls") wb = new HSSFWorkbook(file);
                else wb = new XSSFWorkbook(file); //.xlsx일 경우

                ISheet sheet = wb.GetSheetAt(0);
                var headerRow = sheet.GetRow(0);

                // 헤더명 → DTO 필드명 매핑 추가
                var colIndexToFieldName = new Dictionary<int, string>();
                for (int i = 0; i < headerRow.LastCellNum; i++)
                {
                    var headerName = headerRow.GetCell(i)?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(headerName)) continue;

                    // 어트리뷰트에서 매핑
                    var prop = typeof(T).GetProperties() // T의 프로퍼티를 읽어야 한다
                        .FirstOrDefault(p => p.GetCustomAttribute<ExcelColumnAttribute>()?.Name == headerName);

                    string colName = prop?.Name ?? headerName; // 매핑 없으면 헤더명 그대로
                    dt.Columns.Add(colName);
                    colIndexToFieldName[i] = colName;
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
