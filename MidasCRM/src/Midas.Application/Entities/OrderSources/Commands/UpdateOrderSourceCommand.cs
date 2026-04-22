using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.OrderSources;

namespace Midas.Application.Entities.OrderSources.Commands
{
    public class UpdateOrderSourceCommand : IRequest<OrderSource>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
    }

    public class UpdateOrderSourceCommandHandler(
        IGetQueries<OrderSource, int> queries,
        IEntityRepository<OrderSource> repository)
        : IRequestHandler<UpdateOrderSourceCommand, OrderSource>
    {
        public async Task<OrderSource> Handle(UpdateOrderSourceCommand request, CancellationToken cancellationToken)
        {
            var orderSource = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (orderSource == null)
            {
                throw new Exception($"OrderSource with id {request.Id} not found.");
            }

            orderSource.Update(request.Name);
            await repository.UpdateAsync(orderSource, cancellationToken);
            return orderSource;
        }
    }
}
