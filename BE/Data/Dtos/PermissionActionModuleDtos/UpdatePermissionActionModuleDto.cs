using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class UpdatePermissionActionModuleDto
    {
        [Required]
        public int actionModuleId { get; set; }
        [Required]
        public int userUpdated { get; set; }
    }
}
