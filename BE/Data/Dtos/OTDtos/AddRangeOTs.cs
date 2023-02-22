using BE.Data.Enum.OTEnums;
using BE.Data.Enum;

namespace BE.Data.Dtos.OTDtos
{
    public class AddRangeOTs
    {
        public int? id { get; set; } = null;
        public Types type { get; set; }
        public DateTime Date { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public string realTime { get; set; }
        public StatusOT status { get; set; }
        public string description { get; set; }
        public int? leadCreate { get; set; }
        public DateTime? dateCreate { get; set; }
        public int? updateUser { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string? note { get; set; }
        public List<int> user { get; set; }
        public int idProject { get; set; }
    }
}
