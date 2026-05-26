using FluentValidation;
using Midas.Application.Entities.Products.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.CommandsValidators
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Id must be greater than 0.");
        }
    }
}
