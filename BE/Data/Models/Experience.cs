using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Experience")]
    public class Experience
    {
        [Key]
        public int id { get; set; }
        public string? taskName { get; set; }
        public decimal? spend { get; set; }
        public decimal? estimate { get; set; }
        public decimal? performance { get; set; }
        public int? reviewId { get; set; }
        [ForeignKey("reviewId")]
        public StaffReview? StaffReview { get; set; }
    }
}
