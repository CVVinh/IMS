using BE.Data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    public class Customer_PaidReason : BaseEntity
    {
        public int idCustomer { get; set; }
        //[ForeignKey("id")]
        //public Customer Customer { get; set; }

        public int idPaidReason { get; set; }
        //[ForeignKey("id")]
        //public PaidReasons paidReason { get; set; }

    }
}
