using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class EduInfoDto
    {
        public string eduId { get; set; }
        public string eduName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int batchNumber { get; set; }
        public int capacity { get; set; }
        public string delYn { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }

        public override string ToString()
        {
            return $"eduId: {eduId}, eduName: {eduName}, startDate: {startDate}" +
                   $"endDate: {endDate}, batchNumber: {batchNumber}, capacity: {capacity}" +
                   $"delYn: {delYn}, createdAt: {createdAt}, updatedAt:{updatedAt}";
        }
    }
}
