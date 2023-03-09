namespace BE.Data.Dtos.StaffReviewDtos
{
    public class CreateStaffReviewDto
    {
        public int reviewerId { get; set; }
        public int? staffReview { get; set; }
        public decimal? totalPerformance { get; set; }
        public string? achievements { get; set; }
        public string? knowledge { get; set; }
        public string? skill { get; set; }
        public string? forwardMindset { get; set; }
        public string? positiveMindset { get; set; }
        public string? steadfastMindset { get; set; }
        public string? perfectionistMindset { get; set; }
        public string? fromTalkToResult { get; set; }
        public string? connectToAction { get; set; }
        public string? hobbies { get; set; }
        public string? personality { get; set; }
        public string? commitmentsForCompany { get; set; }
        public string? colleagueOpinion { get; set; }
        public string? HROpinion { get; set; }
        public ICollection<CreateExperienceDto>? experiences { get; set; }
        public CreateReviewResultDto? ReviewResult { get; set; }
    }
}
