namespace BE.Data.Dtos.InfoDeviceDtos
{
    public class CreateInfoDeviceDto
    {
        public int UserId { get; set; }
        public string DeviceName { get; set; }
        public string OperatingSystem { get; set; }
        public string SystemType { get; set; }
        public List<ApplicationDto>? Application { get; set; }
    }
}
