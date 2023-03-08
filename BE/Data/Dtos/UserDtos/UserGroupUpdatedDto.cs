using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UserDtos
{
    public class UserGroupUpdatedDto
    {
        [Required]
        public int idGroup { get; set; }

        [Required]
        public int? userUpdated { get; set; }
    }
}
