using BE.Data.Contexts;
using BE.Data.Enum.LeaveOff;
using FluentValidation;

namespace BE.Data.Dtos.LeaveOffDtos.Validator
{
    public class AddNewLeaveOffDtoValidator: AbstractValidator<AddNewLeaveOffDto>
    {
        private readonly CommonHelperDtos _commonHelperDtos;
        public AddNewLeaveOffDtoValidator(AppDbContext context,CommonHelperDtos commonHelperDtos)
        {
            _commonHelperDtos = commonHelperDtos;
            RuleFor(x => x.idLeaveUser).NotEmpty().WithMessage("idLeaveUser is not empty")
                                    .Must(_commonHelperDtos.userExist).WithMessage("idLeaveUser doesn't exist !");
            RuleFor(x => x.startTime).NotEmpty().WithMessage("startTime is not empty");
            RuleFor(x => x.endTime).NotEmpty().WithMessage("endTime is not empty");
            RuleFor(x => x.reasons).NotEmpty().WithMessage("Reason is not empty");
            RuleFor(x => x).Custom((request, context) =>
            {
                #region test startTaskDate and endTaskDate
                try
                {
                    if (request.startTime >= request.endTime)
                    {
                        context.AddFailure("Starting date must is smaller than starting date");
                    }
                }
                catch
                {
                    context.AddFailure("Starting time or ending time isn't in the correct format !");
                }
                #endregion
            });
        }

    }
}
