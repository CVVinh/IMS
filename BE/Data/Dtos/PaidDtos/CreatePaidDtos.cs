namespace BE.Data.Dtos.PaidDtos
{
    public class CreatePaidDtos
    {
        public int PaidPerson { get; set; }
        public int? ProjectId { get; set; }
        public string CustomerName { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaidReason { get; set; }
        public ICollection<IFormFile>? paidImage { get; set; }
    }
}
