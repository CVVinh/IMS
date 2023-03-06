using System.ComponentModel.DataAnnotations;

namespace BE.Data.Dtos.UserDtos
{
    public class UserGroupDeletedDto
    {
        [Required]
        public int idUser { get; set; }

        [Required]
        public int idGroup { get; set; }

        [Required]
        public int userDeleted { get; set; }
    }
}
