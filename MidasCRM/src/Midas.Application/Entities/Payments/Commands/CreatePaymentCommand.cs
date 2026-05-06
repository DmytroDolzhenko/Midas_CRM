using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.Payments;

namespace Midas.Application.Entities.Payments.Commands
{
    public class CreatePaymentCommand : ICommand<Payment>
    {
        public required int OrderId { get; init; }
        public required decimal Amount { get; init; }
        public required PaymentMethods Method { get; init; }
    }

    public class CreatePaymentCommandHandler(IEntityRepository<Payment> repository)
        : IRequestHandler<CreatePaymentCommand, Payment>
    {
        public async Task<Payment> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = Payment.Create(
                request.OrderId,
                request.Amount,
                request.Method);

            await repository.AddAsync(payment, cancellationToken);
            return payment;
        }
    }
}
