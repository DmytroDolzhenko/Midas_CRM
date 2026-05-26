using FluentValidation;
using Midas.Application.Entities.Users.Commands;
using Midas.Core.Enums;

namespace Midas.Application.Entities.Users.CommandsValidators
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");

            RuleFor(x => x.Surname)
                .NotEmpty()
                .WithMessage("Surname is required")
                .MaximumLength(100)
                .WithMessage("Surname must be less than 100 characters");

            RuleFor(x => x.Fathername)
                .NotEmpty()
                .WithMessage("Fathername is required")
                .MaximumLength(100)
                .WithMessage("Fathername must be less than 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is required")
                .EmailAddress()
                .WithMessage("Email format is invalid");

            RuleFor(x => x.Role)
                .Must(role => role == UserRole.Admin || role == UserRole.Operator)
                .WithMessage("Role is invalid");
        }
    }
}
