using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.ProductCategoriesDtoValidators
{
    public class CreateProductCategoryDtoValidator : AbstractValidator<CreateProductCategoryDto>
    {
        public CreateProductCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class UpdateProductCategoryNameDtoValidator : AbstractValidator<UpdateProductCategoryNameDto>
    {
        public UpdateProductCategoryNameDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }

    public class DeleteProductCategoryDtoValidator : AbstractValidator<DeleteProductCategoryDto>
    {
        public DeleteProductCategoryDtoValidator()
        {
            RuleFor(x => x.IsDeleted)
                .Equal(true)
                .WithMessage("IsDeleted must be true.");
        }
    }
}
