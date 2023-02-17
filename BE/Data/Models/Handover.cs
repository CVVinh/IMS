using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BE.Data.Enum.Handover.Status;

namespace BE.Data.Models
{
    [Table("Handover")]
    public class Handover
    {
        [Key]
        public int IdHandover { get; set; }
        [Required]
        public int IdDevice { get; set; }
        [Required]
        public int UserReceive { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public StatusHandover Status { get; set; }
        [Required]
        public int UserCreated { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        public int UserUpdated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }
        [Required]
        public int IsDeleted { get; set; }
    }
}
