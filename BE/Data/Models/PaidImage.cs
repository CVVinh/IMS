using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("PaidImage")]
    public class PaidImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImagePath { get; set; }
        public int PaidId { get; set; }
        [ForeignKey("PaidId")]
        public Paid Paid { get; set; }
    }
}
