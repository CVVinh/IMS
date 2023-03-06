using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.ActionModuleDtos
{
    public class DeleteActionModuleDto
    {
        [Required]
        public int userDeleted { get; set; }
    }
}
