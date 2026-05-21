using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.Enums;
using Midas.Core.FinancialOperations;

namespace Midas.Application.Entities.FinancialOperations.Commands
{
    public class CreateFinancialOperationCommand : ICommand<FinancialOperation>
    {
        public required FinancialOperationType OperationType { get; init; }
        public required FinancialOperationCategory Category { get; init; }
        public required decimal Amount { get; init; }
        public string? Comment { get; init; }
        public Guid? OrderId { get; init; }
    }

    public class CreateFinancialOperationCommandHandler(
        IEntityRepository<FinancialOperation> repository,
        IGetQueries<Company, Guid> companyQueries,
        IEntityRepository<Company> companyRepository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateFinancialOperationCommand, FinancialOperation>
    {
        public async Task<FinancialOperation> Handle(CreateFinancialOperationCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var userId = currentUserService.UserId;
            var company = await companyQueries.GetByIdAsync(companyId, cancellationToken)
                ?? throw new Exception($"Company with id {companyId} not found.");

            var operation = FinancialOperation.Create(
                companyId,
                request.OperationType,
                request.Category,
                request.Amount,
                request.Comment,
                request.OrderId,
                userId);

            company.ApplyFinancialOperation(request.OperationType, request.Amount);

            await repository.AddAsync(operation, cancellationToken);
            await companyRepository.UpdateAsync(company, cancellationToken);
            return operation;
        }
    }
}
