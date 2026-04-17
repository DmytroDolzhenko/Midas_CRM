using FluentValidation;
using Midas.Application.Entities.Orders.Commands;

namespace Midas.Application.Entities.Orders.CommandsValidators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than 0.");

            RuleFor(x => x.Adress)
            .NotNull()
            .WithMessage("Adress is required.");
        }
    }
}
