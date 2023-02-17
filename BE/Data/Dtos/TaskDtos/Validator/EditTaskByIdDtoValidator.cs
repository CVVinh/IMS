using BE.Data.Contexts;
using FluentValidation;

namespace BE.Data.Dtos.TaskDtos.Validator
{
    public class EditTaskByIdDtoValidator: AbstractValidator<EditTaskByIdDto>
    {
        private readonly AppDbContext _context;
        public EditTaskByIdDtoValidator(AppDbContext context)
        {
            _context = context;
            RuleFor(x => x.taskName).NotEmpty().WithMessage("Task name is not empty")
                 .MaximumLength(50).WithMessage("Task name can not 50 characters");
            RuleFor(x => x.status).NotEmpty().WithMessage("Status is not empty");
            RuleFor(x => x.idProject).NotEmpty().WithMessage("idProject is not empty")
                                    .Must(projectExist).WithMessage("idProject doesn't exist !");
            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.startTaskDate.HasValue && request.endTaskDate.HasValue)
                {
                    if (request.startTaskDate > request.endTaskDate)
                    {
                        context.AddFailure("Starting date must is smaller than starting date");
                    }
                }

                #region test assignee in Project
                if (request.assignee.HasValue)
                {
                    var testAssignee = userProjectExist(request.assignee, request.idProject);
                    if (testAssignee < 0)
                    {
                        context.AddFailure("assignee isn't in the correct format !");
                    }
                    if (testAssignee == 0)
                    {
                        context.AddFailure("assignee doesn't exist in project");
                    }
                }
                #endregion
            });
        }

        #region testing functions
        // test user existed in project
        private int userProjectExist(int? idUser, int idProject)
        {
            try
            {
                var user = _context.Member_Projects.Where(u => u.member == idUser &&
                                                               u.idProject == idProject &&
                                                               u.isDeleted == false).Count();
                if (user > 0)
                {
                    return 1;
                }
                return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        // test project Existed 
        private bool projectExist(int idProject)
        {
            var project = _context.Projects.Where(p => p.Id == idProject && p.IsDeleted == false).FirstOrDefault();
            if (project is null)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
