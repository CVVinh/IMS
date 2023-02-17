using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Dtos.RulesDTOs
{
    public class AddOrUpdateRulesDTO
    {
        public string title { get; set; }
        public string? content { get; set; }
        public DateTime? applyDay { get; set; }
        public DateTime? expiredDay { get; set; }
        public int idUser { get; set; }
        public IFormFile? formFile { get; set; }
    }
}
