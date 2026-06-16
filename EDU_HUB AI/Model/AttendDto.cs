using EDU_HUB_AI.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class AttendDto
    {
        public string attendanceId { get; set; }
        [ExcelColumn("교육생ID")] // 엑셀 header와 매핑
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string phone { get; set; } 
        public string eduId { get; set; }
        public string eduName { get; set; }
        public string eduStartDate { get; set; }
        public string eduEndDate { get; set; }
        [ExcelColumn("출석상태")]
        public string status { get; set; }
        [ExcelColumn("사유")]
        public string? message { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        [ExcelColumn("출석일자")]
        public string attendDate { get; set; }

        public override string ToString()
        {
            return $"AttendanceId: {attendanceId}, studentId: {studentId}, studentName: {studentName}, phone: {phone} " +
                   $"eduId: {eduId}, eduName: {eduName}, eduStartDate: {eduStartDate}, " +
                   $"eduEndDate: {eduEndDate}, status: {status}, message: {message} " +
                   $"attendDate: {attendDate}, createdAt: {createdAt}, updatedAt: {updatedAt}";
        }
    }
}
