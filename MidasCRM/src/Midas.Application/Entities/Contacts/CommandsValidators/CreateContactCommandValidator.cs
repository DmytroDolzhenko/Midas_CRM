using FluentValidation;
using Midas.Application.Entities.Contacts.Commands;

namespace Midas.Application.Entities.Contacts.CommandsValidators
{
    public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
    {
        public CreateContactCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Value is required")
                .MaximumLength(255)
                .WithMessage("Value must be less than 255 characters");
        }
    }
}
