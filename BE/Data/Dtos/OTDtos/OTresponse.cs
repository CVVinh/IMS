using BE.Data.Models;

namespace BE.Data.Dtos.OTDtos
{
    public class OTresponse
    {
        public List<OTs> OTs { get; set; } = new List<OTs>();
        public int Pages { get; set; }
        public int CurrentPage { get; set; }
        public List<Devices> Device { get; internal set; }
    }
}
