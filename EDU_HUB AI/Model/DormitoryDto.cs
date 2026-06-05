using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class DormitoryDto
    {
        public string dormitoryId { get; set; }
        public string eduId { get; set; }
        public int currentCount { get; set; }
        public int maxCount { get; set; }
        public string delYn { get; set; }
        public string dormitoryRoomName { get; set; }

        public override string ToString()
        {
            return $"dormitoryId: {dormitoryId}, eduId:{eduId}, currentCount: {currentCount}" +
                   $"maxCount: {maxCount}, delYn: {delYn}, dormitoryRoomName: {dormitoryRoomName}";
        }
    }
}
