using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.FinancialOperations;

namespace Midas.Application.Entities.FinancialOperations.Commands
{
    public class UpdateFinancialOperationCommand : ICommand<FinancialOperation>
    {
        public required Guid Id { get; init; }
        public required FinancialOperationType OperationType { get; init; }
        public required FinancialOperationCategory Category { get; init; }
        public required decimal Amount { get; init; }
        public string? Comment { get; init; }
        public Guid? OrderId { get; init; }
    }

    public class UpdateFinancialOperationCommandHandler(
        IGetQueries<FinancialOperation, Guid> queries,
        IEntityRepository<FinancialOperation> repository)
        : IRequestHandler<UpdateFinancialOperationCommand, FinancialOperation>
    {
        public async Task<FinancialOperation> Handle(UpdateFinancialOperationCommand request, CancellationToken cancellationToken)
        {
            var operation = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (operation is null)
            {
                throw new Exception($"Financial operation with id {request.Id} not found.");
            }

            operation.Update(request.OperationType, request.Category, request.Amount, request.Comment, request.OrderId);
            await repository.UpdateAsync(operation, cancellationToken);
            return operation;
        }
    }
}
