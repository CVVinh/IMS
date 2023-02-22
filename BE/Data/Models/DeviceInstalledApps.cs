using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE.Data.Models
{
    [Table("DeviceInstalledApps")]
    public class DeviceInstalledApps
    {
        [Key]
        public int ApplicationId { get; set; }
        public string? ApplicationName { get; set; }
        public string? ApplicationLocation { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}
