using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Payments;

namespace Midas.Application.Entities.Payments.Commands
{
    public class DeletePaymentCommand : ICommand<Payment>
    {
        public required Guid Id { get; init; }
    }

    public class DeletePaymentCommandHandler(
        IGetQueries<Payment, Guid> queries,
        IEntityRepository<Payment> repository)
        : IRequestHandler<DeletePaymentCommand, Payment>
    {
        public async Task<Payment> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (payment == null)
            {
                throw new Exception($"Payment with id {request.Id} not found.");
            }

            await repository.DeleteAsync(payment, cancellationToken);
            return payment;
        }
    }
}
