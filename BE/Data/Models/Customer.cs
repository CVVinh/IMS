using BE.Data.Models.Base;

namespace BE.Data.Models
{
    public class Customer : BaseEntity
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public byte? gender { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? company { get; set; }
        public string? address { get; set; }
        public byte workStatus { get; set; }
        public string? identityCard { get; set; }
        public string? accountNumber { get; set; }

        public string fullName
        {
            get { return $"{lastName} {firstName}"; }
        }

        //public ICollection<Customer_PaidReason>? customerPaidReasons { get; set; }
    }
}
