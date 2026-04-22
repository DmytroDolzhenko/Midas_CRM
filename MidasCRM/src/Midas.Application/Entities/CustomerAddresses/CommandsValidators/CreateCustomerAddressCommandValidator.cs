using FluentValidation;
using Midas.Application.Entities.CustomerAddresses.Commands;

namespace Midas.Application.Entities.CustomerAddresses.CommandsValidators
{
    public class CreateCustomerAddressCommandValidator : AbstractValidator<CreateCustomerAddressCommand>
    {
        public CreateCustomerAddressCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("CustomerId must be greater than 0.");

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City is required")
                .MaximumLength(100)
                .WithMessage("City must be less than 100 characters");

            RuleFor(x => x.PostalCode)
                .GreaterThan(0)
                .WithMessage("PostalCode must be greater than 0.");

            RuleFor(x => x.PostDepartmentNumber)
                .GreaterThan(0)
                .WithMessage("PostDepartmentNumber must be greater than 0.");
        }
    }
}
