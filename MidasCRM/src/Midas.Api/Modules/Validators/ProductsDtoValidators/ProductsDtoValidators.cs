using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.ProductsDtoValidators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .WithMessage("Warehouse Id must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(500)
                .WithMessage("Description must be less than 500 characters");

            RuleFor(x => x.ProductCategoryId)
                .GreaterThan(0)
                .WithMessage("Product Category Id must be greater than 0");
        }
    }

    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Description is required")
                .MaximumLength(500)
                .WithMessage("Description must be less than 500 characters");
        }
    }

    public class UpdateProductCategoryDtoValidator : AbstractValidator<UpdateProductCategoryDto>
    {
        public UpdateProductCategoryDtoValidator()
        {
            RuleFor(x => x.ProductCategoryId)
                .GreaterThan(0)
                .WithMessage("Product Category Id must be greater than 0.");
        }
    }

    public class ChangeWarehouseDtoValidator : AbstractValidator<ChangeWarehouseDto>
    {
        public ChangeWarehouseDtoValidator()
        {
            RuleFor(x => x.NewWarehouseId)
                .GreaterThan(0)
                .WithMessage("Warehouse Id must be greater than 0.");
        }
    }

    public class DeleteProductDtoValidator : AbstractValidator<DeleteProductDto>
    {
        public DeleteProductDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("Is Deleted must be true.");
        }
    }
}
