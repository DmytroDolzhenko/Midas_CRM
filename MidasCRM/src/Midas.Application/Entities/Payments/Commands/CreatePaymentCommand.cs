using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Payments;

namespace Midas.Application.Entities.Payments.Commands
{
    public class CreatePaymentCommand : ICommand<Payment>
    {
        public required Guid OrderId { get; init; }
        public required decimal Amount { get; init; }
        public required PaymentMethods Method { get; init; }
    }

    public class CreatePaymentCommandHandler(
        IEntityRepository<Payment> repository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreatePaymentCommand, Payment>
    {
        public async Task<Payment> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var payment = Payment.Create(
                request.OrderId,
                request.Amount,
                request.Method,
                companyId);

            await repository.AddAsync(payment, cancellationToken);
            return payment;
        }
    }
}

