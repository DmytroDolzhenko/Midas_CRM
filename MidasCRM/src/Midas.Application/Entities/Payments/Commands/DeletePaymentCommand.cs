using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.Payments;

namespace Midas.Application.Entities.Payments.Commands
{
    public class DeletePaymentCommand : IRequest<Payment>
    {
        public required int Id { get; init; }
    }

    public class DeletePaymentCommandHandler(
        IGetQueries<Payment> queries,
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
