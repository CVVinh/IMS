using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.DeviceDto
{
    public class UpdateDeviceDto
    {
        [Required]
        public int IdDevice { get; set; }
        [Required]
        public string? DeviceName { get; set; }
        [Required]
        public string? Info { get; set; }
        [Required]
        public int UserUpdated { get; set; }
        [Required]
        public string? Note { get; set; }
    }
}
