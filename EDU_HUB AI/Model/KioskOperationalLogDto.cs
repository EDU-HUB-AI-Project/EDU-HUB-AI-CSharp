using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class KioskOperationalLogDto
    {
        public string opLogId { get; set; }
        public string studentId { get; set; }
        public string printing {  get; set; }
        public string dorm {  get; set; }
        public string createdAt { get; set; }
        public string delYN {  get; set; }

        public override string ToString()
        {
            return $"opLogId: {opLogId}, studentId: {studentId}, printing: {printing}, " +
                   $"dorm: {dorm}, createdAt: {createdAt}, delYN: {delYN}";
        }
    }
}
