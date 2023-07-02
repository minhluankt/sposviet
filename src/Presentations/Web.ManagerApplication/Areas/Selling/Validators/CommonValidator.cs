using Domain.Entities;
using FluentValidation;

namespace Web.ManagerApplication.Areas.Selling.Validators
{
    public class VietQRValidator : AbstractValidator<VietQR>
    {
        public VietQRValidator()
        {
            RuleFor(p => p.IdBankAccount)
                   .NotEmpty().WithMessage("Vui lòng chọn tài khoản ngân hàng")
                   .NotNull();
            RuleFor(p => p.Template)
                   .NotEmpty().WithMessage("Vui lòng chọn template")
                   .NotNull();
        }
    }
}
