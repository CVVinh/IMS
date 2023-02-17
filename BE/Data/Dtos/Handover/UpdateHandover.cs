using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.Handover
{
    public class UpdateHandover
    {
        [Required]
        public int IdHandover { get; set; }
        [Required]
        public int IdDevice { get; set; }
        [Required]
        public int UserReceive { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int UserUpdated { get; set; }
        [Required]
        public DateTime DateUpdated { get; set; }
    }
}
