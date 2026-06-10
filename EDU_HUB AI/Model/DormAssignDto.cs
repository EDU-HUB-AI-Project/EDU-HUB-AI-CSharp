namespace EDU_HUB_AI.Model
{
    public class DormAssignDto
    {
        public string? studentId { get; set; }
        public string? studentName { get; set; }
        public string? eduId { get; set; }
        public string? phone { get; set; }
        public string? dormitoryId { get; set; }
        public string? dormitoryRoomName { get; set; }
        public string? assignStatus { get; set; }

        public override string ToString()
        {
            return $"studentId: {studentId}, studentName: {studentName}, eduId: {eduId}, " +
                   $"phone: {phone}, dormitoryId: {dormitoryId}, dormitoryRoomName: {dormitoryRoomName}, " +
                   $"assignStatus: {assignStatus}";
        }
    }
}
