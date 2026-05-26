using FluentValidation;
using Midas.Application.Entities.Customers.Commands;

namespace Midas.Application.Entities.Customers.CommandsValidators
{
    public class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
    {
        public DeleteCustomerCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
