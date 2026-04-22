using Api.Dtos;
using FluentValidation;
using Midas.Api.Modules.Validators.CustomerAdressesDtoValidators;

namespace Midas.Api.Modules.Validators.OrdersDtoValidators
{
    public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
    {
        public CreateOrderDtoValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0)
                .WithMessage("Customer Id must be greater than 0.");

            RuleFor(x => x.Address)
                .NotNull()
                .WithMessage("Address is required.")
                .SetValidator(new CreateCustomerAddressDtoValidator());
        }
    }

    public class DeleteOrderDtoValidator : AbstractValidator<DeleteOrderDto>
    {
        public DeleteOrderDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }

    public class AddItemToOrderDtoValidator : AbstractValidator<AddItemToOrderDto>
    {
        public AddItemToOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order id is required.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0)
                .WithMessage("ProductVariantId must be greater than 0.");
        }
    }

    public class RemoveItemFromOrderDtoValidator : AbstractValidator<RemoveItemFromOrderDto>
    {
        public RemoveItemFromOrderDtoValidator()
        {
            RuleFor(x => x.OrderId)
                .NotEmpty()
                .WithMessage("Order id is required.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.OrderItemId)
                .GreaterThan(0)
                .WithMessage("OrderItemId must be greater than 0.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.ProductVariantId)
                .GreaterThan(0)
                .WithMessage("ProductVariantId must be greater than 0.");
        }
    }
}
