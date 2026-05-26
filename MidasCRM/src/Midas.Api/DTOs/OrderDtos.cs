using Midas.Core.Enums;
using Midas.Core.Orders;

namespace Api.Dtos
{
    public record OrderDto(
        Guid Id,
        string UniqCode,
        int CustomerId,
        CustomerAddressDto? Address,
        ServiceType ServiceType,
        OrderStatus Status,
        decimal TotalCost,
        string Description,
        DateTime CreatedAt,
        Guid CompanyId,
        string? TrackingNumber,
        IReadOnlyCollection<OrderItemDto> OrderItems,
        PaymentMethods PaymentMethods,
        bool IsDeleted
    )
    {
        public static OrderDto FromDomain(Order order)
            => new(
                order.Id,
                order.UniqCode,
                order.CustomerId,
                order.Address is null ? null : CustomerAddressDto.FromDomain(order.Address),
                order.ServiceType,
                order.Status,
                order.TotalCost,
                order.Description,
                order.CreatedAt,
                order.CompanyId,
                order.TrackingNumber,
                order.OrderItems.Select(OrderItemDto.FromDomain).ToList(),
                order.PaymentMethods,
                order.IsDeleted
            );
    }

    public record CreateOrderDto(
        int CustomerId,
        CreateCustomerAddressDto Address,
        ServiceType ServiceType,
        CargoType CargoType,
        PaymentMethods PaymentMethods,
        string Description
    );

    public record CreateOrderOneClickDto(
        CreateOneClickCustomerDto Customer,
        CreateOneClickAddressDto Address,
        ServiceType ServiceType,
        CargoType CargoType,
        string Description,
        IReadOnlyCollection<CreateOneClickOrderItemDto> Items,
        PaymentMethods PaymentMethods
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
        int PostDepartmentNumber,
        DeliveryPointType DeliveryPointType
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
    public record UpdateOrderStatusDto(
        Guid OrderId,
        OrderStatus Status
    );
}

