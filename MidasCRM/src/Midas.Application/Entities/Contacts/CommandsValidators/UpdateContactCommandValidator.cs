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

            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required")
                .MaximumLength(255)
                .WithMessage("Value must be less than 255 characters");
        }
    }
}
