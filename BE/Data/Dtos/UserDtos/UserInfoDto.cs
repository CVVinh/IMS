using BE.Data.Models;

namespace BE.Data.Dtos.UserDtos
{
    public class UserInfoDto
    {
        public int id { get; set; }
        public string userCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public UserInfoDto(Users user)
        {
            this.id = user.id;
            this.userCode = user.userCode;
            this.firstName = user.firstName;
            this.lastName = user.lastName; 
        }

    }
}
