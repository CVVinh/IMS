using BE.Data.Enum;

namespace BE.Data.Dtos.OTDtos
{
    public class AcceptOTDto
    {
        public int id { get; set; }
        public StatusOT status { get; set; }
        public string? note { get; set; }
        public int PM { get; set; }
    }
}
