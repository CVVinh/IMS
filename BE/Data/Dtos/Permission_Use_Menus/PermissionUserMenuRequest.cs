using ServiceStack.DataAnnotations;

namespace BE.Data.Dtos.Permission_Use_Menus
{
    public class PermissionUserMenuRequest
    {
        [Required]
        public int idModule { get; set; }
        [Required]
        public int IdUser { get; set; }
        [Required]
        public int idGroup { get; set; }
    }
}
