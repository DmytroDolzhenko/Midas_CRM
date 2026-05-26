using FluentValidation;
using Midas.Application.Entities.CompanyMembers.Commands;

namespace Midas.Application.Entities.CompanyMembers.CommandsValidators
{
    public class RemoveCompanyMemberCommandValidator : AbstractValidator<RemoveCompanyMemberCommand>
    {
        public RemoveCompanyMemberCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
