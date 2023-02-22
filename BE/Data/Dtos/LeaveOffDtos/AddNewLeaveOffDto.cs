namespace BE.Data.Dtos.LeaveOffDtos
{
    public class AddNewLeaveOffDto
    {
        public int idLeaveUser { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string reasons { get; set; }

        public int? idCompanyBranh { get; set; }
    }
}
