namespace BE.Data.Dtos.LeaveOffDtos
{
    public class FindByNameStatusDateDtos
    {
        public string? fullName { get; set; }
        public DateTime? date { get; set; }
        public List<int>? status { get; set; }
    }
}
