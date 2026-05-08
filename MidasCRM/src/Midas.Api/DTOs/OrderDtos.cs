using Microsoft.Identity.Client;
using Midas.Core.Enums;
using Midas.Core.Orders;

namespace Api.Dtos
{
    public record OrderDto(
        Guid Id,
        string UniqCode,
        int CustomerId,
        CustomerAddressDto Address,
        OrderStatus Status,
        decimal TotalCost,
        DateTime CreatedAt,
        Guid OwnerId,
        IReadOnlyCollection<OrderItemDto> OrderItems,
        bool IsDeleted
    )
    {
        public static OrderDto FromDomain(Order order)
            => new(
                order.Id,
                order.UniqCode,
                order.CustomerId,
                CustomerAddressDto.FromDomain(order.Address),
                order.Status,
                order.TotalCost,
                order.CreatedAt,
                order.OwnerId,
                order.OrderItems.Select(OrderItemDto.FromDomain).ToList(),
                order.IsDeleted
            );
    }

    public record CreateOrderDto(
        int CustomerId,
        CreateCustomerAddressDto Address
    );

    public record CreateOrderOneClickDto(
        CreateOneClickCustomerDto Customer,
        CreateOneClickAddressDto Address,
        IReadOnlyCollection<CreateOneClickOrderItemDto> Items
    );

    public record CreateOneClickCustomerDto(
        string Name,
        string Surname,
        string ContactValue,
        string Email
    );

    public record CreateOneClickAddressDto(
        string City,
        int PostalCode,
        int PostDepartmentNumber
    );

    public record CreateOneClickOrderItemDto(
        int ProductVariantId,
        int Quantity
    );

    public record DeleteOrderDto(
        bool IsDeleted
    );

    public record AddItemToOrderDto(
        Guid OrderId,
        int ProductId,
        int Quantity,
        int ProductVariantId
    );

    public record RemoveItemFromOrderDto(
        Guid OrderId,
        int ProductId,
        int OrderItemId,
        int Quantity,
        int ProductVariantId
    );
}
