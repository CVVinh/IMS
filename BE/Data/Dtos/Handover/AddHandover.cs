using System.ComponentModel.DataAnnotations;
using static BE.Data.Enum.Handover.Status;

namespace BE.Data.Dtos.Handover
{
    public class AddHandover
    {
        [Required]
        public int IdDevice { get; set; }
        [Required]
        public int UserReceive { get; set; }
        [Required]
        public int Amount { get; set; }
        [Required]
        public int UserCreated { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
    }
}
