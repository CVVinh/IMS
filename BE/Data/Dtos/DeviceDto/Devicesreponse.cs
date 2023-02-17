using BE.Data.Models;

namespace BE.Data.Dtos.DeviceDto
{
    public class Devicesreponse
    {
        public List<Devices> Devices { get; set; } = new List<Devices>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
        /*public List<Devices> Device { get; internal set; }*/
    }
}
