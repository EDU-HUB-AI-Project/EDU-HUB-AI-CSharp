using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class StudentSearchDto
    {
        public string studentId {  get; set; }
        public string birthDate { get; set; }
        public string dormitoryId { get; set; }
        public string phoneNumber { get; set; }

        public override string ToString()
        {
            return $"studentId: {studentId}, birthDate: {birthDate}" +
                   $"dormitoryId: {dormitoryId}, phoneNumber: {phoneNumber}";
        }
    }
}
