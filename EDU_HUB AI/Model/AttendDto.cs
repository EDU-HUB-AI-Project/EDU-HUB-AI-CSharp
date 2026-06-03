using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class AttendDto
    {
        public string AttendanceId { get; set; }
        public string studentId { get; set; }
        public string eduId { get; set; }
        public string status { get; set; }
        public string message { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string attendDate { get; set; }

        public override string ToString()
        {
            return $"AttendanceId: {AttendanceId}, studentId: {studentId}, eduId: {eduId}, " +
                   $"status: {status}, message: {message}, attendDate: {attendDate}, " +
                   $"createdAt: {createdAt}, updatedAt: {updatedAt}";
        }
    }
}
