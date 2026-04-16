using FluentValidation;
using Midas.Application.Entities.Users.Commands;

namespace Midas.Application.Entities.Users.CommandsValidators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
