using BE.Data.Contexts;
using FluentValidation;

namespace BE.Data.Dtos.MemberProjectDtos.Validator
{
    public class AddMemberDtoValidator: AbstractValidator<AddMemberDto>
    {
        private readonly AppDbContext _context;
        public AddMemberDtoValidator(AppDbContext context)
        {
            _context = context;
            RuleFor(u => u.createUser).Must(userExist).WithMessage("createUser doesn't exist !!!");
            RuleFor(u => u.member).Must(userExist).WithMessage("Member doesn't exist !!!");
            RuleFor(u => u.idProject).Must(projectExist).WithMessage("idProject doesn't exist !!!");
            RuleFor(x => x).Custom((request, context) =>
            {
                var testMember = userProjectExist(request.member, request.idProject);
                if (testMember == true)
                {
                    context.AddFailure("Member existed in project");
                }

            });
        }

        private bool userExist(int idUser)
        {
            try
            {
                var user =  _context.Users.Where( u => u.id == idUser && u.workStatus == 1).FirstOrDefault();
                if(user is null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        // test user existed in project
        private bool userProjectExist(int idUser, int idProject)
        {
            try
            {
                var user = _context.Member_Projects.Where(u => u.member == idUser && 
                                                                u.idProject == idProject && 
                                                                u.isDeleted == false).FirstOrDefault();
                if (user is null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool projectExist(int idProject)
        {
            try
            {
                var project = _context.Projects.Where(p => p.IsDeleted == false && p.Id == idProject).FirstOrDefault();
                if (project is null)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
