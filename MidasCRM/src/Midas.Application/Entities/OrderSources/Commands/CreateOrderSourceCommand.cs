using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.OrderSources;

namespace Midas.Application.Entities.OrderSources.Commands
{
    public class CreateOrderSourceCommand : ICommand<OrderSource>
    {
        public required string Name { get; init; }
    }

    public class CreateOrderSourceCommandHandler(IEntityRepository<OrderSource> repository)
        : IRequestHandler<CreateOrderSourceCommand, OrderSource>
    {
        public async Task<OrderSource> Handle(CreateOrderSourceCommand request, CancellationToken cancellationToken)
        {
            var orderSource = OrderSource.Create(0, request.Name);
            await repository.AddAsync(orderSource, cancellationToken);
            return orderSource;
        }
    }
}
