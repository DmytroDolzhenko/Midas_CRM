using FluentValidation;
using Midas.Application.Entities.Payments.Commands;

namespace Midas.Application.Entities.Payments.CommandsValidators
{
    public class DeletePaymentCommandValidator : AbstractValidator<DeletePaymentCommand>
    {
        public DeletePaymentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Payment id is required.");
        }
    }
}
