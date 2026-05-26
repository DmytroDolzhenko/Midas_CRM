using FluentValidation;
using Midas.Application.Entities.Companies.Commands;

namespace Midas.Application.Entities.Companies.CommandsValidators
{
    public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
    {
        public UpdateCompanyCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TaxNumber).MaximumLength(50);
        }
    }
}
