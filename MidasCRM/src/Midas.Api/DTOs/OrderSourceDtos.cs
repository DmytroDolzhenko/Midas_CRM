using Midas.Core.OrderSources;

namespace Api.Dtos
{
    public record OrderSourceDto(
        int Id,
        string Name,
        bool IsDeleted
    )
    {
        public static OrderSourceDto FromDomain(OrderSource orderSource)
            => new(
                orderSource.Id,
                orderSource.Name,
                orderSource.IsDeleted
            );
    }

    public record CreateOrderSourceDto(
        string Name
    );

    public record UpdateOrderSourceDto(
        string Name
    );

    public record DeleteOrderSourceDto(
        bool IsDeleted
    );
}
