using Domain.Entities;
using FluentValidation;
using FluentValidation.Validators;
using Model;

namespace Web.ManagerApplication.Areas.Admin.Validators
{
    //https://docs.fluentvalidation.net/en/latest/conditions.html
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
            RuleFor(p => p.Email).EmailAddress(EmailValidationMode.Net4xRegex)
                .When(customer => !string.IsNullOrEmpty(customer.Email), ApplyConditionTo.CurrentValidator)
                .WithMessage("Email không đúng định dạng");
            RuleFor(p => p.Name)
                   .NotEmpty().When(customer => customer.TypeCustomer==Application.Enums.ENumTypeCustomer.Company, ApplyConditionTo.CurrentValidator)
                   .WithMessage("Khách hàng là doanh nghiệp vui lòng nhập tên đơn vị"); 
            RuleFor(p => p.Taxcode)
                   .NotEmpty().When(customer => customer.TypeCustomer==Application.Enums.ENumTypeCustomer.Company, ApplyConditionTo.CurrentValidator)
                   .WithMessage("Khách hàng là doanh nghiệp vui lòng nhập mã số thuế");
            RuleFor(p => p.Buyer)
                   .NotEmpty().When(customer => customer.TypeCustomer == Application.Enums.ENumTypeCustomer.Personal, ApplyConditionTo.CurrentValidator)
                   .WithMessage("Khách hàng là cá nhân vui lòng nhập tên người mua  hàng");

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
