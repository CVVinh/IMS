using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ActionModuleDtos
{
    public class EditActionModuleDto
    {
        [Required]
        public string name { get; set; }
        [Required]
        public string description { get; set; }
        [Required]
        public int userUpdated { get; set; }
    }
}
