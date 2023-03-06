using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ModuleDtos
{
    public class ModuleDtos
    {
        [Required]
        public string nameModule { get; set; }
        public string note { get; set; }
        public int idSort { get; set; } = 0;
    }
}
