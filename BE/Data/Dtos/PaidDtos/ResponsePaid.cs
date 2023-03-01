using BE.Data.Models;

namespace BE.Data.Dtos.PaidDtos
{
    public class ResponsePaid
    {
        public int id { get; set; }

        public int paidPerson { get; set; }
        public string paidPersonName { get; set; }

        public int? personConfirm { get; set; }
        public int? personConfirmName { get; set; }

        public DateTime paidDate { get; set; }

        public int? projectId { get; set; }
        public string? nameProject { get; set; }
        public bool isDelPro { get; set; }

        public int customerName { get; set; }
        public string customerFullName { get; set; }

        public decimal amountPaid { get; set; }

        public int paidReason { get; set; }
        public string paidReasonName { get; set; }

        public string? contentReason { get; set; }
        public bool isPaid { get; set; } // 1 is paid, 0 is unpaid
        public List<PaidImage>? paidImages { get; set; }
    }
}
