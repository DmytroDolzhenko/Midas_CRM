using FluentValidation;
using Midas.Application.Entities.Payments.Commands;
using Midas.Core.Enums;

namespace Midas.Application.Entities.Payments.CommandsValidators
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("OrderId must be greater than 0.");

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
}
