using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.GruopDtos
{
    public class ChangePermissionGroupDto
    {
        [Required]
        public int IdModule { get; set; }
        [Required]
        public bool Access { get; set; } = true;
    }
}
