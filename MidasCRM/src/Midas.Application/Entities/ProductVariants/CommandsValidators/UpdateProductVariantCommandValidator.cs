using FluentValidation;
using Midas.Application.Entities.ProductVariants.Commands;

namespace Midas.Application.Entities.ProductVariants.CommandsValidators
{
    public class UpdateProductVariantCommandValidator : AbstractValidator<UpdateProductVariantCommand>
    {
        public UpdateProductVariantCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0.");

            RuleFor(x => x.UniqCode)
                .NotEmpty()
                .WithMessage("UniqCode is required")
                .MaximumLength(100)
                .WithMessage("UniqCode must be less than 100 characters");

            RuleFor(x => x.Color)
                .NotEmpty()
                .WithMessage("Color is required")
                .MaximumLength(50)
                .WithMessage("Color must be less than 50 characters");

            RuleFor(x => x.Size)
                .NotEmpty()
                .WithMessage("Size is required")
                .MaximumLength(20)
                .WithMessage("Size must be less than 20 characters");

            RuleFor(x => x.CostPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CostPrice must be greater than or equal to 0.");

            RuleFor(x => x.SellPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("SellPrice must be greater than or equal to 0.");
        }
    }
}
