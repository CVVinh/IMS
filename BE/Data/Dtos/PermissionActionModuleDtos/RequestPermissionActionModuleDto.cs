using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.PermissionActionModuleDtos
{
    public class RequestPermissionActionModuleDto
    {
        [Required]
        public int idModule { get; set; }

        [Required]
        public int idAction { get; set; }
    }
}
