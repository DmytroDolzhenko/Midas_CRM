using FluentValidation;
using Midas.Application.Entities.Customers.Commands;

namespace Midas.Application.Entities.Customers.CommandsValidators
{
    public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
    {
        public CreateCustomerCommandValidator()
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

            RuleFor(x => x.ContactValue)
                .NotEmpty()
                .WithMessage("ContactValue is required")
                .MaximumLength(255)
                .WithMessage("ContactValue must be less than 255 characters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email must be greater than 0.");
        }
    }
}
