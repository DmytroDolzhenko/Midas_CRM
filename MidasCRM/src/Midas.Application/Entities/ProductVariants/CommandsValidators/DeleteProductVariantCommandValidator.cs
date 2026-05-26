using FluentValidation;
using Midas.Application.Entities.ProductVariants.Commands;

namespace Midas.Application.Entities.ProductVariants.CommandsValidators
{
    public class DeleteProductVariantCommandValidator : AbstractValidator<DeleteProductVariantCommand>
    {
        public DeleteProductVariantCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
