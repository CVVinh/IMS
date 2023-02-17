using BE.Data.Contexts;
using BE.Data.Enums.TaskEnums;
using BE.Data.Models;
using BE.Response;
using FluentValidation;

namespace BE.Data.Dtos.TaskDtos.Validator
{
    public class AddNewTaskDtoValidator: AbstractValidator<AddNewTaskDto>
    {
        private readonly AppDbContext _context;
        public AddNewTaskDtoValidator(AppDbContext context)
        {
            _context = context;
            RuleFor(x => x.taskName).NotEmpty().WithMessage("Task name is not empty")
                .MaximumLength(50).WithMessage("Task name can not 50 characters");
            RuleFor(x => x.idProject).NotEmpty().WithMessage("idProject is not empty")
                                    .Must(projectExist).WithMessage("idProject doesn't exist !");
            RuleFor(x => x).Custom((request, context) =>
            {
                #region test startTaskDate and endTaskDate
                if (request.startTaskDate.HasValue && request.endTaskDate.HasValue)
                {
                    try
                    {
                        DateTime startTaskDate = request.startTaskDate.Value;
                        DateTime endTaskDate = request.endTaskDate.Value;
                        if (request.startTaskDate > request.endTaskDate)
                        {
                            context.AddFailure("Starting date must is smaller than starting date");
                        }
                    }
                    catch
                    {
                        context.AddFailure("Starting date or ending date isn't in the correct format !");
                    }
                }
                #endregion

                #region test createUser in Project
                var testcreateUser = userProjectExist(request.createUser, request.idProject);
                if (testcreateUser < 0)
                {
                    context.AddFailure("createUser isn't in the correct format !");
                }
                if (testcreateUser == 0)
                {
                    context.AddFailure("createUser doesn't exist in project");
                }
                #endregion

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
