using Domain.Entities;
using FluentValidation;
using Model;

namespace Web.ManagerApplication.Areas.Admin.Validators
{
    public class ProductModelViewValidator : AbstractValidator<ProductModelView>
    {
        public ProductModelViewValidator()
        {
            RuleFor(p => p.Name)
                   .NotEmpty().WithMessage("Vui lòng nhập tên sản phẩm")
                   .NotNull();
          
            RuleFor(p => p.idCategory)
                   .NotEmpty().WithMessage("Vui lòng chọn danh mục sản phẩm")
                   .NotNull();
        }
    }
    public class CustomerModelValidator : AbstractValidator<CustomerModel>
    {
        [Obsolete]
        public CustomerModelValidator()
        {
            //  RuleFor(p => p.Email).EmailAddress(EmailValidationMode.Net4xRegex)
            RuleFor(p => p.Name)
                   .NotEmpty().WithMessage("Vui lòng nhập tên")
                   .NotNull();
        }
    }public class PaymentMethodValidator : AbstractValidator<PaymentMethod>
    {
        
        public PaymentMethodValidator()
        {
            //  RuleFor(p => p.Email).EmailAddress(EmailValidationMode.Net4xRegex)
            RuleFor(p => p.Name)
                   .NotEmpty().WithMessage("Vui lòng nhập tên")
                   .NotNull();
            RuleFor(p => p.Code)
                   .NotEmpty().WithMessage("Vui lòng nhập mã hình thức thanh toán")
                   .NotNull();
        }
    }
}
