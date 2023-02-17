using BE.Data.Enum;
using BE.Data.Enum.OTEnums;
using System.ComponentModel.DataAnnotations;

namespace BE.Data.Models
{
    public class OTs
    {
        [Key]
        public int id { get; set; }
        public Types type { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string start { get; set; }
        [Required]
        public string end { get; set; }
        public string realTime { get; set; }
        public StatusOT status { get; set; }
        public string description { get; set; }
        public int? leadCreate { get; set; }
        public DateTime? dateCreate { get; set; }
        public int? updateUser { get; set; }
        public DateTime? dateUpdate { get; set; }
        public string? note { get; set; }
        public int user { get; set; }
        public int idProject { get; set; }

    }
}
