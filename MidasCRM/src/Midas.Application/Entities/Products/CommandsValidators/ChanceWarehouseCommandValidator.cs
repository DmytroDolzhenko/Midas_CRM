using FluentValidation;
using Midas.Application.Entities.Products.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Products.CommandsValidators
{
    public class ChanceWarehouseCommandValidator : AbstractValidator<ChangeWarehouseCommand>
    {
        public ChanceWarehouseCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.");
            RuleFor(x => x.NewWarehouseId)
                .GreaterThan(0).WithMessage("NewWarehouseId must be greater than 0.");
        }
    }
}
