using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.GruopDtos
{
    public class PermissionGroupDto
    {
        [Required]
        public int IdGroup { get; set; }
        [Required]
        public int IdModule { get; set; }
        public bool Access { get; set; } = true;
    }
}
