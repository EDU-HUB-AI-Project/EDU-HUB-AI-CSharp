using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class DormInOutDto
    {
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string dormitoryId { get; set; }
        public string dormitoryRoomName { get; set; }
        public string dorm { get; set; }

        public override string ToString()
        {
            return $"studentId: {studentId}, studentName: {studentName}, " +
                $"dormitoryId:{dormitoryId}, dormitoryRoomName: {dormitoryRoomName}  dorm: {dorm}";
        }
    }
}
