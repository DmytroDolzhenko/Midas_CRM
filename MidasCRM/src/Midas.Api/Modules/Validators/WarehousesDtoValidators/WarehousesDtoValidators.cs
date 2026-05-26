using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.WarehousesDtoValidators
{
    public class CreateWarehouseDtoValidator : AbstractValidator<CreateWarehouseDto>
    {
        public CreateWarehouseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDto>
    {
        public UpdateWarehouseDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class AddProductToWarehouseDtoValidator : AbstractValidator<AddProductToWarehouseDto>
    {
        public AddProductToWarehouseDtoValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .WithMessage("WarehouseId must be greater than 0");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0");
        }
    }

    public class RemoveProductFromWarehouseDtoValidator : AbstractValidator<RemoveProductFromWarehouseDto>
    {
        public RemoveProductFromWarehouseDtoValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .WithMessage("WarehouseId must be greater than 0");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0");
        }
    }
}
