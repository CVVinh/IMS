using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class AddPermissionActionModuleDto
    {
        [Required]
        public int moduleId { get; set; }
        [Required]
        public int actionModuleId { get; set; }
        [Required]
        public int userCreated { get; set; }
    }
}
