using FluentValidation;
using Midas.Application.Entities.Users.Commands;

namespace Midas.Application.Entities.Users.CommandsValidators
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("User id is required.");

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
        }
    }
}
