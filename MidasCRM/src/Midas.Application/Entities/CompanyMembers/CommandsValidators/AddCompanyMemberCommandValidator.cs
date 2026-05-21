using FluentValidation;
using Midas.Application.Entities.CompanyMembers.Commands;

namespace Midas.Application.Entities.CompanyMembers.CommandsValidators
{
    public class AddCompanyMemberCommandValidator : AbstractValidator<AddCompanyMemberCommand>
    {
        public AddCompanyMemberCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Role).IsInEnum();
        }
    }
}
