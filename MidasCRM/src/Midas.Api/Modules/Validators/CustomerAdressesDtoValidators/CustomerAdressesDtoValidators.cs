using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.CustomerAdressesDtoValidators
{
    public class CreateCustomerAddressDtoValidator : AbstractValidator<CreateCustomerAddressDto>
    {
        public CreateCustomerAddressDtoValidator()
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

    public class UpdateCustomerAddressDtoValidator : AbstractValidator<UpdateCustomerAddressDto>
    {
        public UpdateCustomerAddressDtoValidator()
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

    public class DeleteCustomerAddressDtoValidator : AbstractValidator<DeleteCustomerAddressDto>
    {
        public DeleteCustomerAddressDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
