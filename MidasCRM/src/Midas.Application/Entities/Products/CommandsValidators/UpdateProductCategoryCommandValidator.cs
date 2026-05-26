using FluentValidation;
using Midas.Application.Entities.Products.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.CommandsValidators
{
    public class UpdateProductCategoryCommandValidator : AbstractValidator<UpdateProductCategoryCommand>
    {
        public UpdateProductCategoryCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required")
                .GreaterThan(0).WithMessage("Id must be greater than 0.");

            RuleFor(x => x.ProductCategoryId)
                .NotEmpty().WithMessage("ProductCategoryId is required")
                .GreaterThan(0).WithMessage("ProductCategoryId must be greater than 0.");
        }
    }
}
