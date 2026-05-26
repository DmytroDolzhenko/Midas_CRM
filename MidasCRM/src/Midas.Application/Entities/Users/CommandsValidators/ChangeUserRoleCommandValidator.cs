using FluentValidation;
using Midas.Application.Entities.Users.Commands;
using Midas.Core.Enums;

namespace Midas.Application.Entities.Users.CommandsValidators
{
    public class ChangeUserRoleCommandValidator : AbstractValidator<ChangeUserRoleCommand>
    {
        public ChangeUserRoleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User id is required.");

            RuleFor(x => x.Role)
                .Must(role => role == UserRole.Admin || role == UserRole.Operator)
                .WithMessage("Role is invalid");
        }
    }
}
