using FluentValidation;
using Midas.Application.Entities.Companies.Commands;

namespace Midas.Application.Entities.Companies.CommandsValidators
{
    public class DeleteCompanyCommandValidator : AbstractValidator<DeleteCompanyCommand>
    {
        public DeleteCompanyCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
