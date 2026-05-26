using Midas.Core.Enums;
using Midas.Core.Payments;

namespace Api.Dtos
{
    public record PaymentDto(
        Guid Id,
        Guid OrderId,
        decimal Amount,
        PaymentMethods Method,
        PaymentStatus Status,
        DateTime CreatedAt,
        bool IsDeleted
    )
    {
        public static PaymentDto FromDomain(Payment payment)
            => new(
                payment.Id,
                payment.OrderId,
                payment.Amount,
                payment.Method,
                payment.Status,
                payment.CreatedAt,
                payment.IsDeleted
            );
    }

    public record CreatePaymentDto(
        Guid OrderId,
        decimal Amount,
        PaymentMethods Method
    );

    public record DeletePaymentDto(
        bool IsDeleted
    );
}
