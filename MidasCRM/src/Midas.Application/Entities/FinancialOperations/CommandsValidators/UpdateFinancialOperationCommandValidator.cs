using FluentValidation;
using Midas.Application.Entities.FinancialOperations.Commands;

namespace Midas.Application.Entities.FinancialOperations.CommandsValidators
{
    public class UpdateFinancialOperationCommandValidator : AbstractValidator<UpdateFinancialOperationCommand>
    {
        public UpdateFinancialOperationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.OperationType).IsInEnum();
            RuleFor(x => x.Category).IsInEnum();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
}
