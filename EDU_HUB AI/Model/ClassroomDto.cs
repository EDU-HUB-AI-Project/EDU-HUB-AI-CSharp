using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class ClassroomDto
    {
        public string classroomId { get; set; }
        public string classroomName { get; set; }
        public int floor { get; set; }
        public string imageId { get; set; }
        public string imagePath { get; set; }
        public string createdAt { get; set; }
        public string updatedAt { get; set; }
    }
}
