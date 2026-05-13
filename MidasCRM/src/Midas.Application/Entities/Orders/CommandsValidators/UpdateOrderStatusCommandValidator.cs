using FluentValidation;
using Midas.Application.Entities.Orders.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Orders.CommandsValidators
{
    public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
    {
        public UpdateOrderStatusCommandValidator()
        {
            RuleFor(x => x.OrderId).NotEmpty();
            RuleFor(x => x.Status).IsInEnum();
        }
    }
}
