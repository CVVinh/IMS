using Microsoft.CodeAnalysis;

namespace BE.Data.Dtos.PaidDtos
{
    public class SearchDayPaidDtos
    {
        public int? id { get; set; } = -1;
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
