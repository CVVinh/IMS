using BE.Data.Enum.OTEnums;

namespace BE.Data.Dtos.OTDtos
{
    public class PaginateOT
    {
        public Types type { get; set; }
        public int page { get; set; }
        public int recordNum{ get; set; }
    }
}
