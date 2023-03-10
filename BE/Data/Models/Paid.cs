using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("Paids")]

    public class Paid
    {
        [Key]
        public int Id { get; set; }
        public int PaidPerson { get; set; }
        public int? PersonConfirm { get; set; }
        /*   [ForeignKey("PaidPerson")]
           public Users Users { get; set; }*/
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime PaidDate { get; set; }
        public int? ProjectId { get; set; }
/*        [ForeignKey("ProjectId")]
        public Project Project { get; set; }*/
        public string CustomerName { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaidReason { get; set; }
        public string? ContentReason { get; set; }
        public bool IsPaid { get; set; } // 1 is paid, 0 is unpaid
        //public bool IsAccept { get; set; } // 1 is Accept, otherwise
        public ICollection<PaidImage>? paidImages { get; set; }
    }
}
