namespace BE.Data.Dtos.StaffReviewDtos
{
    public class ExperienceDto
    {
        public int id { get; set; }
        public string? taskName { get; set; }
        public decimal? spend { get; set; }
        public decimal? estimate { get; set; }
        public decimal? performance { get; set; }
        public int? reviewId { get; set; }
    }
}
