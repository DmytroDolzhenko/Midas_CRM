using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.OrderItemsDtoValidators
{
    public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order id is required.");

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

    public class UpdateOrderItemDtoValidator : AbstractValidator<UpdateOrderItemDto>
    {
        public UpdateOrderItemDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order id is required.");

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

    public class DeleteOrderItemDtoValidator : AbstractValidator<DeleteOrderItemDto>
    {
        public DeleteOrderItemDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
