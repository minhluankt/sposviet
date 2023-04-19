using Domain.ViewModel;
using FluentValidation;
using FluentValidation.Validators;
using HelperLibrary;

namespace Web.ManagerCompany.ValidateForms
{

    public class ValidateCompanyModelValidator : AbstractValidator<CompanyAdminInfoViewModel>
    {
        public ValidateCompanyModelValidator()
        {
            RuleFor(p => p.Name)
                   .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm")
                   .NotNull();
            RuleFor(p => p.PhoneNumber).MinimumLength(10).MaximumLength(11).WithMessage("Số điện thoại không đúng định dạng")
                  .NotEmpty().WithMessage("Vui lòng nhập số điện thoại")
                  .NotNull();
            RuleFor(p => p.CusTaxCode).Custom((value, context) =>
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (!LibraryCommon.CheckMST(value))
                    {
                        context.AddFailure("Mã số thuế không hợp lệ");
                    }
                }
                
            });

            RuleFor(p => p.CusTaxCode)
                .Matches("(?!.@#])\\A\\S+\\z").WithMessage("Mã số thuế không hợp lệ")
                .MaximumLength(20).WithMessage("Mã số thuế không hợp lệ");

            RuleFor(p => p.Address)
                  .NotEmpty().WithMessage("Vui lòng nhập địa chỉ")
                  .NotNull();
            RuleFor(p => p.Email).EmailAddress(EmailValidationMode.Net4xRegex);

            RuleFor(p => p.StartDate)
                   .NotEmpty().WithMessage("Vui lòng chọn ngày bắt đầu hoạt động")
                   .NotNull();
            RuleFor(p => p.NumberDateExpiration).GreaterThanOrEqualTo(1).WithMessage("Vui lòng chọn đăng ký gói")
                   .NotEmpty().WithMessage("Vui lòng chọn đăng ký gói")
                   .NotNull();
            RuleFor(p => (int)p.IdDichVu).GreaterThanOrEqualTo(0).WithMessage("Vui lòng chọn loại dịch vụ")
                  .NotEmpty().WithMessage("Vui lòng chọn loại dịch vụ")
                  .NotNull();
        }
    }
}
