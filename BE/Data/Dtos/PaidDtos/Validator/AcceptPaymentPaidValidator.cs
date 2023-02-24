using BE.Data.Dtos.LeaveOffDtos;
using FluentValidation;

namespace BE.Data.Dtos.PaidDtos.Validator
{
    public class AcceptPaymentPaidValidator : AbstractValidator<AcceptPaymentPaidDtos>
    {
        public AcceptPaymentPaidValidator() 
        {
            RuleFor(x => x.PaidPerson).NotEmpty().WithMessage("PaidPerson is not empty");
        }

    }
}
