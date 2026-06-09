using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDU_HUB_AI.Model
{
    public class StudentDto
    {
        public string studentId { get; set; }
        public string studentName { get; set; }
        public string birthDate { get; set; }
        public string eduId { get; set; }
        public string attendYn { get; set; }
        public string dormYn { get; set; }
        public string dormitoryId { get; set; }
        public string delYn { get; set; }
        public string createdAt { get; set; }
        public string phoneNumber { get; set; }

        public override string ToString()
        {
            return $"studentId: {studentId}, studentName: {studentName}, birthDate: {birthDate}, " +
                   $"eduId: {eduId}, attendYn: {attendYn}, dormYn: {dormYn}, dormitoryId: {dormitoryId}, " +
                   $"delYn: {delYn}, createdAt: {createdAt}, phoneNumber: {phoneNumber}";
        }
    }
}
