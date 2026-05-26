using Api.Dtos;
using FluentValidation;
using Midas.Core.Enums;

namespace Midas.Api.Modules.Validators.UsersDtoValidators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
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

            RuleFor(x => x.Role)
                .Must(role => role == UserRole.Admin || role == UserRole.Operator)
                .WithMessage("Role is invalid");
        }
    }

    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
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

    public class DeleteUserDtoValidator : AbstractValidator<DeleteUserDto>
    {
        public DeleteUserDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }

    public class ApproveUserDtoValidator : AbstractValidator<ApproveUserDto>
    {
        public ApproveUserDtoValidator()
        {
            RuleFor(x => x.IsApproved)
                .Equal(true)
                .WithMessage("IsApproved must be true.");
        }
    }

    public class ChangeUserRoleDtoValidator : AbstractValidator<ChangeUserRoleDto>
    {
        public ChangeUserRoleDtoValidator()
        {
            RuleFor(x => x.NewRole)
                .Must(role => role == UserRole.Admin || role == UserRole.Operator)
                .WithMessage("Role is invalid");
        }
    }
}
