using Api.Dtos;
using FluentValidation;
using Midas.Core.Enums;

namespace Midas.Api.Modules.Validators.PaymentsDtoValidators
{
    public class CreatePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymentDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("Order Id must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Method)
                .Must(method =>
                    method == PaymentMethods.FullPayment
                    || method == PaymentMethods.AfterPayment
                    || method == PaymentMethods.PartialPayment)
                .WithMessage("Method is invalid");
        }
    }

    public class DeletePaymentDtoValidator : AbstractValidator<DeletePaymentDto>
    {
        public DeletePaymentDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
