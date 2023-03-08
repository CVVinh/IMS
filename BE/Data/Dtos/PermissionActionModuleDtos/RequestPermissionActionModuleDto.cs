using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class RequestPermissionActionModuleDto
    {
        [Required]
        public int moduleId { get; set; }

        [Required]
        public int actionModuleId { get; set; }
    }
}
