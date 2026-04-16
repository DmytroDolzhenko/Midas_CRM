using FluentValidation;
using Midas.Application.Entities.CustomerAdresses.Commands;

namespace Midas.Application.Entities.CustomerAdresses.CommandsValidators
{
    public class DeleteCustomerAdressCommandValidator : AbstractValidator<DeleteCustomerAdressCommand>
    {
        public DeleteCustomerAdressCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
