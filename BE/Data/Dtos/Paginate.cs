using BE.Data.Enum;

namespace BE.Data.Dtos
{
    public class Paginate
    {
        public int pageIndex { get; set; }
        public int pageSizeEnum { get; set; }
        public string? word { get; set; }
    }
}