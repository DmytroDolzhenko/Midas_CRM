using FluentValidation;
using Midas.Application.Entities.FinancialOperations.Commands;

namespace Midas.Application.Entities.FinancialOperations.CommandsValidators
{
    public class DeleteFinancialOperationCommandValidator : AbstractValidator<DeleteFinancialOperationCommand>
    {
        public DeleteFinancialOperationCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
