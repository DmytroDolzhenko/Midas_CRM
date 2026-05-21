using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.CompaniesDtoValidators
{
    public class CreateCompanyDtoValidator : AbstractValidator<CreateCompanyDto>
    {
        public CreateCompanyDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TaxNumber).MaximumLength(50);
        }
    }

    public class UpdateCompanyDtoValidator : AbstractValidator<UpdateCompanyDto>
    {
        public UpdateCompanyDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TaxNumber).MaximumLength(50);
        }
    }

    public class AddCompanyMemberDtoValidator : AbstractValidator<AddCompanyMemberDto>
    {
        public AddCompanyMemberDtoValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }

    public class UpdateCompanyMemberRoleDtoValidator : AbstractValidator<UpdateCompanyMemberRoleDto>
    {
        public UpdateCompanyMemberRoleDtoValidator()
        {
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}
