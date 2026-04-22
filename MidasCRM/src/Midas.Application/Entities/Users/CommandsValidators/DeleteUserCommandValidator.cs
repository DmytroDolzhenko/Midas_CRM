using FluentValidation;
using Midas.Application.Entities.Users.Commands;

namespace Midas.Application.Entities.Users.CommandsValidators
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User id is required.");
        }
    }
}
