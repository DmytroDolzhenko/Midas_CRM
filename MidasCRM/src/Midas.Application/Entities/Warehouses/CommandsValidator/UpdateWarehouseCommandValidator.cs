using FluentValidation;
using Midas.Application.Entities.Warehouses.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Warehouses.CommandsValidator
{
    public class UpdateWarehouseCommandValidator : AbstractValidator<UpdateWarehouseCommand>
    {
        public UpdateWarehouseCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");
        }
    }
}
