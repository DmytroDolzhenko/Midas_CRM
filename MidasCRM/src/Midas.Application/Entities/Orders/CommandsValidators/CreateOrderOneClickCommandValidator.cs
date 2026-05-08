using FluentValidation;
using Midas.Application.Entities.Orders.Commands;

namespace Midas.Application.Entities.Orders.CommandsValidators
{
    public class CreateOrderOneClickCommandValidator : AbstractValidator<CreateOrderOneClickCommand>
    {
        public CreateOrderOneClickCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CustomerSurname)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.CustomerContactValue)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.CustomerEmail)
                .NotEmpty();

            RuleFor(x => x.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PostalCode)
                .GreaterThan(0);

            RuleFor(x => x.PostDepartmentNumber)
                .GreaterThan(0);

            RuleFor(x => x.Items)
                .NotEmpty();

            RuleForEach(x => x.Items)
                .SetValidator(new CreateOrderOneClickCommandItemValidator());
        }
    }

    public class CreateOrderOneClickCommandItemValidator : AbstractValidator<CreateOrderOneClickCommandItem>
    {
        public CreateOrderOneClickCommandItemValidator()
        {
            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0);

            RuleFor(x => x.Quantity)
                .GreaterThan(0);
        }
    }
}
