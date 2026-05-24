using Midas.Core.CustomerAddresses;
using Midas.Core.Enums;

namespace Api.Dtos
{
    public record CustomerAddressDto(
        int Id,
        int CustomerId,
        string City,
        int PostDepartmentNumber,
        DeliveryPointType DeliveryPointType,
        bool IsDeleted
    )
    {
        public static CustomerAddressDto FromDomain(CustomerAddress customerAdress)
            => new(
                customerAdress.Id,
                customerAdress.CustomerId,
                customerAdress.City,
                customerAdress.PostDepartmentNumber,
                customerAdress.DeliveryPointType,
                customerAdress.IsDeleted
            );
    }

    public record CreateCustomerAddressDto(
        int CustomerId,
        string City,
        int PostDepartmentNumber,
        DeliveryPointType DeliveryPointType
    );

    public record UpdateCustomerAddressDto(
        int CustomerId,
        string City,
        int PostDepartmentNumber,
        DeliveryPointType DeliveryPointType
    );

    public record DeleteCustomerAddressDto(
        bool IsDeleted
    );
}
