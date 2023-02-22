using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("InfoDevices")]
    public class InfoDevices
    {
        [Key]
        public int DeviceId { get; set; }
        public int? UserId { get; set; }
        public string? DeviceName { get; set; }
        public string? OperatingSystem { get; set; }
        public string? SystemType { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
