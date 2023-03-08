using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class DeleteMultiPermissionActionModuleDto
    {
        [Required]
        public int moduleId { get; set; }

        [Required]
        public int actionModuleId { get; set; }

        [Required]
        public int userDeleted { get; set; }
    }
}
