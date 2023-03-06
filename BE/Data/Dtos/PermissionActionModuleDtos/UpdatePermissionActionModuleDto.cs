using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class UpdatePermissionActionModuleDto
    {
        [Required]
        public int idModule { get; set; }
        [Required]
        public int idAction { get; set; }
        [Required]
        public int userUpdated { get; set; }
    }
}
