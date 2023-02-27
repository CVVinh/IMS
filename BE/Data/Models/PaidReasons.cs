using BE.Data.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    public class PaidReasons : BaseEntity
    {
        public string name { get; set; }


        //public ICollection<Customer_PaidReason>? customerPaidReasons { get; set; }
    }
}
