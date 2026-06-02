using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class EduInfoDto
    {
        private string eduId { get; set; }
        private string eduName { get; set; }
        private string startDate { get; set; }
        private string endDate { get; set; }
        private int batchNumber { get; set; }
        private int capacity { get; set; }
        private string delYn { get; set; }
    }
}
