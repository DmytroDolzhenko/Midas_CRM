using FluentValidation;
using Midas.Application.Entities.OrderItems.Commands;

namespace Midas.Application.Entities.OrderItems.CommandsValidators
{
    public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
    {
        public CreateOrderItemCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0)
                .WithMessage("OrderId must be greater than 0.");

            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0)
                .WithMessage("ProductVariantId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0)
                .WithMessage("UnitPrice must be greater than 0.");

            RuleFor(x => x.CostPriceSnapshot)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CostPriceSnapshot must be greater than or equal to 0.");
        }
    }
}
