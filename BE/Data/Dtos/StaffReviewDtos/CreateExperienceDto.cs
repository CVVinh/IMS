namespace BE.Data.Dtos.StaffReviewDtos
{
    public class CreateExperienceDto
    {
        public string? taskName { get; set; }
        public decimal? spend { get; set; }
        public decimal? estimate { get; set; }
        public decimal? performance { get; set; }
    }
}