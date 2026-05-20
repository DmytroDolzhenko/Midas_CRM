using System.Text.Json.Serialization;

namespace Midas.Application.DTO.NovaPoshta
{
    public record GetContactPersonsRequest(
        [property: JsonPropertyName("Ref")] string Ref,
        [property: JsonPropertyName("Page")] string Page = "1");

    public record NpContactPersonItem(string Ref, string Description, string Phones);
    public record NpContactDto(string Ref, string FullName, string Phone);
}
