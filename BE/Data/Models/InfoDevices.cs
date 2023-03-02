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
        public decimal? DeviceTotalRamSize { get; set; }
       /* public string? DeviceRamManufacturer { get; set; }
        public decimal? CapacityPerRam { get; set; }*/
        public string? CpuName { get; set; }
        public string? MainName { get; set; }
        public decimal? DiskDriveTotalSize { get; set; }
        public string? OperatingSystem { get; set; }
        public string? SystemType { get; set; }
        public string? UpdateAt { get; set; }
    }
}
