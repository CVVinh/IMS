using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ModuleDtos
{
    public class addModuleDtos
    {
        [Required]
        //public int Id { get; set; }
        public string nameModule { get; set; }
        public string note { get; set; }
    }
}
