using Midas.Core.Contacts;

namespace Api.Dtos
{
    public record ContactDto(
        int Id,
        string PhoneNumber,
        bool IsDeleted
    )
    {
        public static ContactDto FromDomain(Contact contact)
            => new(
                contact.Id,
                contact.PhoneNumber,
                contact.IsDeleted
            );
    }

    public record CreateContactDto(
        string PhoneNumber
    );

    public record UpdateContactDto(
        string PhoneNumber
    );

    public record DeleteContactDto(
        bool IsDeleted
    );
}
