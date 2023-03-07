using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.GruopDtos
{
    public class PermissionGroupRequestDto
    {
        [Required]
        public int IdGroup { get; set; }
        [Required]
        public int IdModule { get; set; }
    }
}
