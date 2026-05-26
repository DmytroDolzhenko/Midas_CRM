using FluentValidation;
using Midas.Application.Entities.CustomerAddresses.Commands;

namespace Midas.Application.Entities.CustomerAdrdesses.CommandsValidators
{
    public class DeleteCustomerAddressCommandValidator : AbstractValidator<DeleteCustomerAddressCommand>
    {
        public DeleteCustomerAddressCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
