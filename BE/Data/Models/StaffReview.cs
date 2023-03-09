using BE.Data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("StaffReview")]
    public class StaffReview : BaseEntity
    {
        public int? reviewerId { get; set; }
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
        public bool? IsConfirm { get; set; }
        public ICollection<Experience>? experiences { get; set; }
        public ReviewResult? ReviewResult { get; set; }
    }
}
