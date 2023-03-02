namespace BE.Data.Dtos.InfoDeviceDtos
{
    public class CreateInfoDeviceDto
    {
        public int UserId { get; set; }
        public string DeviceName { get; set; }
        public string OperatingSystem { get; set; }
        public string SystemType { get; set; }
        public decimal DeviceTotalRamSize { get; set; }
        public string CpuName { get; set; }
        public string MainName { get; set; }
        public decimal DiskDriveTotalSize { get; set; }
        public List<ApplicationDto>? Application { get; set; }
        public string UpdateAt { get; set; }
    }
}
