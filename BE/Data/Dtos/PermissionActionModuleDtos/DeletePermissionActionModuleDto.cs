using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class DeletePermissionActionModuleDto
    {
        [Required]
        public int userDeleted { get; set; }
    }
}
