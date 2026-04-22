using Midas.Core.CustomerAddresses;

namespace Api.Dtos
{
    public record CustomerAddressDto(
        int Id,
        int CustomerId,
        string City,
        int PostalCode,
        int PostDepartmentNumber,
        bool IsDeleted
    )
    {
        public static CustomerAddressDto FromDomain(CustomerAddress customerAdress)
            => new(
                customerAdress.Id,
                customerAdress.CustomerId,
                customerAdress.City,
                customerAdress.PostalCode,
                customerAdress.PostDepartmentNumber,
                customerAdress.IsDeleted
            );
    }

    public record CreateCustomerAddressDto(
        int CustomerId,
        string City,
        int PostalCode,
        int PostDepartmentNumber
    );

    public record UpdateCustomerAddressDto(
        int CustomerId,
        string City,
        int PostalCode,
        int PostDepartmentNumber
    );

    public record DeleteCustomerAddressDto(
        bool IsDeleted
    );
}
