using FluentValidation;
using Midas.Application.Entities.OrderSources.Commands;

namespace Midas.Application.Entities.OrderSources.CommandsValidators
{
    public class UpdateOrderSourceCommandValidator : AbstractValidator<UpdateOrderSourceCommand>
    {
        public UpdateOrderSourceCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }
}
