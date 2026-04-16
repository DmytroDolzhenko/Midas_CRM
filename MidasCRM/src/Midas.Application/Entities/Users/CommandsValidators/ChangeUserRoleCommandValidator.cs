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
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Role)
                .Must(role => role == UserRole.Admin || role == UserRole.Operator)
                .WithMessage("Role is invalid");
        }
    }
}
