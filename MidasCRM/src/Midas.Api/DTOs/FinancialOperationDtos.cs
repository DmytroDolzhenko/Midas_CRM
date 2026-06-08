using Midas.Core.Enums;
using Midas.Core.FinancialOperations;

namespace Api.Dtos
{
    public record FinancialOperationDto(
        Guid Id,
        Guid CompanyId,
        FinancialOperationType OperationType,
        FinancialOperationCategory Category,
        decimal Amount,
        string? Comment,
        Guid? OrderId,
        Guid? CreatedByUserId,
        string? CreatedByUserEmail,
        DateTime CreatedAt,
        bool IsDeleted)
    {
        public static FinancialOperationDto FromDomain(FinancialOperation operation)
            => new(
                operation.Id,
                operation.CompanyId,
                operation.OperationType,
                operation.Category,
                operation.Amount,
                operation.Comment,
                operation.OrderId,
                operation.CreatedByUserId,
                operation.CreatedByUser?.Email,
                operation.CreatedAt,
                operation.IsDeleted);
    }

    public record CreateFinancialOperationDto(
        FinancialOperationType OperationType,
        FinancialOperationCategory Category,
        decimal Amount,
        string? Comment,
        Guid? OrderId);

    public record UpdateFinancialOperationDto(
        FinancialOperationType OperationType,
        FinancialOperationCategory Category,
        decimal Amount,
        string? Comment,
        Guid? OrderId);

    public record DeleteFinancialOperationDto(bool IsDeleted);
}
