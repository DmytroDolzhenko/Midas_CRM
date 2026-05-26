using Midas.Core.OrderItems;

namespace Api.Dtos
{
    public record OrderItemDto(
        int Id,
        Guid OrderId,
        int ProductVariantId,
        int Quantity,
        decimal UnitPrice,
        decimal CostPriceSnapshot,
        bool IsDeleted
    )
    {
        public static OrderItemDto FromDomain(OrderItem orderItem)
            => new(
                orderItem.Id,
                orderItem.OrderId,
                orderItem.ProductVariantId,
                orderItem.Quantity,
                orderItem.UnitPrice,
                orderItem.CostPriceSnapshot,
                orderItem.IsDeleted
            );
    }

    public record CreateOrderItemDto(
        Guid OrderId,
        int ProductVariantId,
        int Quantity,
        decimal UnitPrice,
        decimal CostPriceSnapshot
    );

    public record UpdateOrderItemDto(
        Guid OrderId,
        int ProductVariantId,
        int Quantity,
        decimal UnitPrice,
        decimal CostPriceSnapshot
    );

    public record DeleteOrderItemDto(
        bool IsDeleted
    );
}
