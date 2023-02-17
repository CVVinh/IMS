using FluentValidation;

namespace BE.Data.Dtos.LeaveOffDtos.Validator
{
    public class AccepterLeaveOffDtoValidator : AbstractValidator<AccepterLeaveOffDto>
    {
        private readonly CommonHelperDtos _commonHelperDtos;
        public AccepterLeaveOffDtoValidator(CommonHelperDtos commonHelperDtos)
        {
            _commonHelperDtos = commonHelperDtos;
            RuleFor(x => x.idAcceptUser).NotEmpty().WithMessage("idAcceptUser is not empty")
                                    .Must(_commonHelperDtos.userExist).WithMessage("idAcceptUser doesn't exist !");
        }
        
    }
}
