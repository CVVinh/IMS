using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BE.Data.Enum.StaffReviewEnums.Contract;

namespace BE.Data.Models
{
    [Table("ReviewResult")]
    public class ReviewResult
    {
        [Key]
        public int id { get; set; }
        public bool? isTerminate { get; set; }
        public AssignContract assignContract { get; set; }
        [ForeignKey("reviewId")]
        public StaffReview? StaffReview { get; set; }
    }
}
