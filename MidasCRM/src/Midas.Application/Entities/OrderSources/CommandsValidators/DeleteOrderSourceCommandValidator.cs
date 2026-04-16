using FluentValidation;
using Midas.Application.Entities.OrderSources.Commands;

namespace Midas.Application.Entities.OrderSources.CommandsValidators
{
    public class DeleteOrderSourceCommandValidator : AbstractValidator<DeleteOrderSourceCommand>
    {
        public DeleteOrderSourceCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
