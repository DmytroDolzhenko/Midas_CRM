using FluentValidation;
using Midas.Application.Entities.Products.Commands;
using Midas.Application.Entities.Warehouses.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Warehouses.CommandsValidator
{
    public class AddProductFromWarehouseCommandValidator : AbstractValidator<AddProductToWarehouseCommand>
    {
        public AddProductFromWarehouseCommandValidator()
        {
            RuleFor(x => x.WarehouseId)
                .GreaterThan(0)
                .WithMessage("WarehouseId must be greater than 0");
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than 0");
        }
    }
}
