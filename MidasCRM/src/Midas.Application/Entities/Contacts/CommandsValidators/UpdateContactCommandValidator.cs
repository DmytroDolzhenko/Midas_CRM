using FluentValidation;
using Midas.Application.Entities.Contacts.Commands;

namespace Midas.Application.Entities.Contacts.CommandsValidators
{
    public class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
    {
        public UpdateContactCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .MaximumLength(13)
                .WithMessage("Phone number must be less than 255 characters");
        }
    }
}
