using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.DeviceDto
{
    public class AddDeviceDto
    {
        [Required]
        public string DeviceName { get; set; } = string.Empty;
        [Required]
        public string Info { get; set; } = string.Empty;
        [Required]
        public int UserCreated { get; set; }
        public string? Note { get; set; } = string.Empty;
    }
}
