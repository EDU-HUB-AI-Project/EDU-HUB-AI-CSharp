using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class SubjectDto
    {
        public string subjectId { get; set; }
        public string eduId { get; set; }
        public string subjectName { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string endYn { get; set; }
        public string eduRoomName { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
        public string delYn { get; set; }
    }
}
