using Midas.Core.Contacts;

namespace Api.Dtos
{
    public record ContactDto(
        int Id,
        string Value,
        bool IsDeleted
    )
    {
        public static ContactDto FromDomain(Contact contact)
            => new(
                contact.Id,
                contact.Value,
                contact.IsDeleted
            );
    }

    public record CreateContactDto(
        string Value
    );

    public record UpdateContactDto(
        string Value
    );

    public record DeleteContactDto(
        bool IsDeleted
    );
}
