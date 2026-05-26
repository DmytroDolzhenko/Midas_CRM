using FluentValidation;
using Midas.Application.Entities.OrderSources.Commands;

namespace Midas.Application.Entities.OrderSources.CommandsValidators
{
    public class CreateOrderSourceCommandValidator : AbstractValidator<CreateOrderSourceCommand>
    {
        public CreateOrderSourceCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }
}
