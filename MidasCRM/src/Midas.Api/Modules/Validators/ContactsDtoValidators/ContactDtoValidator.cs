using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.ContactsDtoValidators
{
    public class ContactDtoValidator : AbstractValidator<CreateContactDto>
    {
        public ContactDtoValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .MaximumLength(13)
                .WithMessage("Phone number must be less than 13 characters");
        }
    }

    public class UpdateContactDtoValidator : AbstractValidator<UpdateContactDto>
    {
        public UpdateContactDtoValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required")
                .MaximumLength(13)
                .WithMessage("Phone number must be less than 13 characters");
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
