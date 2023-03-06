using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class AddPermissionActionModuleDto
    {
        [Required]
        public int idModule { get; set; }
        [Required]
        public int idAction { get; set; }
        [Required]
        public int userCreated { get; set; }
    }
}
