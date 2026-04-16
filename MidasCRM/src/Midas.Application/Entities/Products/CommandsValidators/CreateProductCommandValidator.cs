using FluentValidation;
using Midas.Application.Entities.Products.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.CommandsValidators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
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

            RuleFor(x => x.ProductCategoryId)
                .GreaterThan(0)
                .WithMessage("ProductCategoryId must be greater than 0");
        }
    }
}
