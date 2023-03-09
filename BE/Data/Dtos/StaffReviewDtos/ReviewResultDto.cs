using static BE.Data.Enum.StaffReviewEnums.Contract;

namespace BE.Data.Dtos.StaffReviewDtos
{
    public class ReviewResultDto
    {
        public int id { get; set; }
        public bool? isTerminate { get; set; }
        public string assignContract { get; set; }
    }
}
