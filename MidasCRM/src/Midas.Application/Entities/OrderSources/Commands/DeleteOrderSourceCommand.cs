using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.OrderSources;

namespace Midas.Application.Entities.OrderSources.Commands
{
    public class DeleteOrderSourceCommand : IRequest<OrderSource>
    {
        public required int Id { get; init; }
    }

    public class DeleteOrderSourceCommandHandler(
        IGetQueries<OrderSource, int> queries,
        IEntityRepository<OrderSource> repository)
        : IRequestHandler<DeleteOrderSourceCommand, OrderSource>
    {
        public async Task<OrderSource> Handle(DeleteOrderSourceCommand request, CancellationToken cancellationToken)
        {
            var orderSource = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (orderSource == null)
            {
                throw new Exception($"OrderSource with id {request.Id} not found.");
            }

            orderSource.MarkAsDelete();
            await repository.UpdateAsync(orderSource, cancellationToken);
            return orderSource;
        }
    }
}
