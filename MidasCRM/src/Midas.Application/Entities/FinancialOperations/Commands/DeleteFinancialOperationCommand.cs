using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.FinancialOperations;

namespace Midas.Application.Entities.FinancialOperations.Commands
{
    public class DeleteFinancialOperationCommand : ICommand<FinancialOperation>
    {
        public required Guid Id { get; init; }
    }

    public class DeleteFinancialOperationCommandHandler(
        IGetQueries<FinancialOperation, Guid> queries,
        IEntityRepository<FinancialOperation> repository)
        : IRequestHandler<DeleteFinancialOperationCommand, FinancialOperation>
    {
        public async Task<FinancialOperation> Handle(DeleteFinancialOperationCommand request, CancellationToken cancellationToken)
        {
            var operation = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (operation is null)
            {
                throw new Exception($"Financial operation with id {request.Id} not found.");
            }

            operation.MarkAsDeleted();
            await repository.UpdateAsync(operation, cancellationToken);
            return operation;
        }
    }
}
