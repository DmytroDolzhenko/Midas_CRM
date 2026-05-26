using FluentValidation;
using Midas.Application.Entities.Companies.Commands;

namespace Midas.Application.Entities.Companies.CommandsValidators
{
    public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
    {
        public CreateCompanyCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.TaxNumber).MaximumLength(50);
        }
    }
}
