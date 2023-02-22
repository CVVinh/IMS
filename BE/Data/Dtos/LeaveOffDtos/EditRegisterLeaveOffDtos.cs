namespace BE.Data.Dtos.LeaveOffDtos
{
    public class EditRegisterLeaveOffDtos
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string reasons { get; set; }

        public int? idCompanyBranh { get; set; }
    }
}
