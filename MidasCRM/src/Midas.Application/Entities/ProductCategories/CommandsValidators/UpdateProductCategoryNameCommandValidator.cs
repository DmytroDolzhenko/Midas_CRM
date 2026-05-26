using FluentValidation;
using Midas.Application.Entities.ProductCategories.Commands;

namespace Midas.Application.Entities.ProductCategories.CommandsValidators
{
    public class UpdateProductCategoryNameCommandValidator : AbstractValidator<UpdateProductCategoryNameCommand>
    {
        public UpdateProductCategoryNameCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }
}
