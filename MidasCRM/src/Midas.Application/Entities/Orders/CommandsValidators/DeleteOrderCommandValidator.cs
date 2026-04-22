using FluentValidation;
using Midas.Application.Entities.Orders.Commands;

namespace Midas.Application.Entities.Orders.CommandsValidators
{
    public class DeleteOrderCommandValidator : AbstractValidator<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Order id is required.");
        }
    }
}
