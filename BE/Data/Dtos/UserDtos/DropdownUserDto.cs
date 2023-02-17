using BE.Data.Models;

namespace BE.Data.Dtos.UsersDTO
{
    public class DropdownUserDto
    {
        public int id { get; set; }
        public string userCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DropdownUserDto(Users user)
        {
            id = user.id;
            userCode = user.userCode;
            firstName = user.firstName;
            lastName = user.lastName;
        }
    }
}
