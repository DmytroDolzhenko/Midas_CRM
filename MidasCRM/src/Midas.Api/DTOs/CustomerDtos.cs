using Midas.Core.Customers;

namespace Api.Dtos
{
    public record CustomerDto(
        int Id,
        string Name,
        string Surname,
        ContactDto Contact,
        int Email,
        bool IsDeleted
    )
    {
        public static CustomerDto FromDomain(Customer customer)
            => new(
                customer.Id,
                customer.Name,
                customer.Surname,
                ContactDto.FromDomain(customer.Contact),
                customer.Email,
                customer.IsDeleted
            );
    }

    public record CreateCustomerDto(
        string Name,
        string Surname,
        string ContactValue,
        int Email
    );

    public record UpdateCustomerDto(
        string Name,
        string Surname,
        string ContactValue,
        int Email
    );

    public record DeleteCustomerDto(
        bool IsDeleted
    );
}
