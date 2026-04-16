using FluentValidation;
using Midas.Application.Entities.OrderItems.Commands;

namespace Midas.Application.Entities.OrderItems.CommandsValidators
{
    public class DeleteOrderItemCommandValidator : AbstractValidator<DeleteOrderItemCommand>
    {
        public DeleteOrderItemCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
