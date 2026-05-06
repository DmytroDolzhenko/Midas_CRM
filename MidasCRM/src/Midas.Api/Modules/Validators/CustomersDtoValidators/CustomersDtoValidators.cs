using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.CustomersDtoValidators
{
    public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
    {
        public CreateCustomerDtoValidator()
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
                .GreaterThan(0)
                .WithMessage("Email must be greater than 0.");
        }
    }

    public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
    {
        public UpdateCustomerDtoValidator()
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
                .GreaterThan(0)
                .WithMessage("Email must be greater than 0.");
        }
    }

    public class DeleteCustomerDtoValidator : AbstractValidator<DeleteCustomerDto>
    {
        public DeleteCustomerDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
