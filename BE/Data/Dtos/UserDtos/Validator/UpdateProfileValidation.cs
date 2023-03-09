using BE.Data.Contexts;
using BE.Data.Dtos.LeaveOffDtos;
using FluentValidation;

namespace BE.Data.Dtos.UserDtos.Validator
{
    public class UpdateProfileValidation : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileValidation(AppDbContext dbContext)
        {
            RuleFor(person => person.phoneNumber)
                .NotEmpty().WithMessage("Vui lòng nhập Số điện thoại của bạn.")
                .Matches(@"^\d{10,11}$").WithMessage("Số điện thoại không hợp lệ, phải có độ dài 10 hoặc 11 chữ số.");

            RuleFor(person => person.firstName)
                .NotEmpty().WithMessage("Vui lòng nhập tên của bạn.")
                .Length(1, 100).WithMessage("Tên của bạn không được quá 100 ký tự.");

            RuleFor(person => person.lastName)
                .NotEmpty().WithMessage("Vui lòng nhập họ của bạn.")
                .Length(1, 100).WithMessage("Họ của bạn không được quá 100 ký tự.");

            RuleFor(person => person.identitycard)
                .NotEmpty().WithMessage("Vui lòng nhập số CMND của bạn.")
                .Matches(@"^\d{9,12}$").WithMessage("Số CMND không hợp lệ, phải chứa đúng 9 hoặc 12 chữ số.");

            RuleFor(person => person.email)
                .NotEmpty().WithMessage("Vui lòng nhập địa chỉ email của bạn.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

            RuleFor(person => person.skype)
                .MaximumLength(50).WithMessage("Tên Skype không được quá 50 ký tự.");

            RuleFor(person => person.yearGraduated)
                .NotEmpty().WithMessage("Vui lòng nhập năm tốt nghiệp.")
                .Must(year => int.TryParse(year?.ToString(), out int _)).WithMessage("Năm tốt nghiệp phải là số.")
                .InclusiveBetween(1980, DateTime.Now.Year).WithMessage("Năm tốt nghiệp phải nằm trong khoảng từ năm 1980 đến năm hiện tại.");
        }
    }
}
