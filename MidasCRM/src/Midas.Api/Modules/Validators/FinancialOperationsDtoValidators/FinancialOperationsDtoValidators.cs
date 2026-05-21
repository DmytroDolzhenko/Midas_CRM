using Api.Dtos;
using FluentValidation;

namespace Midas.Api.Modules.Validators.FinancialOperationsDtoValidators
{
    public class CreateFinancialOperationDtoValidator : AbstractValidator<CreateFinancialOperationDto>
    {
        public CreateFinancialOperationDtoValidator()
        {
            RuleFor(x => x.OperationType).IsInEnum();
            RuleFor(x => x.Category).IsInEnum();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }

    public class UpdateFinancialOperationDtoValidator : AbstractValidator<UpdateFinancialOperationDto>
    {
        public UpdateFinancialOperationDtoValidator()
        {
            RuleFor(x => x.OperationType).IsInEnum();
            RuleFor(x => x.Category).IsInEnum();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }

    public class DeleteFinancialOperationDtoValidator : AbstractValidator<DeleteFinancialOperationDto>
    {
        public DeleteFinancialOperationDtoValidator()
        {
            RuleFor(x => x.IsDeleted).Equal(true);
        }
    }
}
