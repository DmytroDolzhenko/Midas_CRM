using FluentValidation;
using Midas.Application.Entities.CompanyMembers.Commands;

namespace Midas.Application.Entities.CompanyMembers.CommandsValidators
{
    public class UpdateCompanyMemberRoleCommandValidator : AbstractValidator<UpdateCompanyMemberRoleCommand>
    {
        public UpdateCompanyMemberRoleCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}
