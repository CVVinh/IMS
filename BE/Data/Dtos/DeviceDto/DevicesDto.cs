using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.DeviceDto
{
    public class CreateDevicesDto
    {
        [Required]
        public string? DeviceName { get; set; }

        public string? Info { get; set; }

        public string? Note { get; set; }

    }
}
