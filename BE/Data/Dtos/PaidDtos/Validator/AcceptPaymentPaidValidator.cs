using BE.Data.Dtos.LeaveOffDtos;
using FluentValidation;

namespace BE.Data.Dtos.PaidDtos.Validator
{
    public class AcceptPaymentPaidValidator : AbstractValidator<AcceptPaymentPaidDtos>
    {
        public AcceptPaymentPaidValidator() 
        {
            RuleFor(x => x.PersonConfirm).NotEmpty().WithMessage("PersonConfirm is not empty");
        }

    }
}
