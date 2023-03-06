using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UserDtos
{
    public class UserGroupUpdatedDto
    {
        [Required]
        public int idUser { get; set; }

        [Required]
        public int idGroup { get; set; }

        [Required]
        public int? userUpdated { get; set; }
    }
}
