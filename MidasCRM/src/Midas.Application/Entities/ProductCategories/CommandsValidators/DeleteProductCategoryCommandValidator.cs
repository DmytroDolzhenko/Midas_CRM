using FluentValidation;
using Midas.Application.Entities.ProductCategories.Commands;

namespace Midas.Application.Entities.ProductCategories.CommandsValidators
{
    public class DeleteProductCategoryCommandValidator : AbstractValidator<DeleteProductCategoryCommand>
    {
        public DeleteProductCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
