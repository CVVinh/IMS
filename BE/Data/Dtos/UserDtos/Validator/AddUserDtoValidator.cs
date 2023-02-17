using BE.Data.Contexts;
using FluentValidation;
namespace BE.Data.Dtos.UsersDTO.Validator
{
    public class AddUserDtoValidator: AbstractValidator<AddUserDto>
    {
        private readonly AppDbContext _context;

        public AddUserDtoValidator(AppDbContext context)
        {
        }

        private bool UniqueUsercode(string usercode)
        {
            var userexist = _context.Users.Where(u => u.userCode == usercode);
            if (userexist.Any())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
