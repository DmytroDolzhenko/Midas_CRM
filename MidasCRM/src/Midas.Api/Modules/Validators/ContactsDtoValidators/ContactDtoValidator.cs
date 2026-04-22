using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.ContactsDtoValidators
{
    public class ContactDtoValidator : AbstractValidator<CreateContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required")
                .MaximumLength(255)
                .WithMessage("Value must be less than 255 characters");
        }
    }

    public class UpdateContactDtoValidator : AbstractValidator<UpdateContactDto>
    {
        public UpdateContactDtoValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .WithMessage("Value is required")
                .MaximumLength(255)
                .WithMessage("Value must be less than 255 characters");
        }
    }

    public class DeleteContactDtoValidator : AbstractValidator<DeleteContactDto>
    {
        public DeleteContactDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
