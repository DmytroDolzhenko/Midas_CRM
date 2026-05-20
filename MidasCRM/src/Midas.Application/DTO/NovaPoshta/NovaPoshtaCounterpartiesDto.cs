using System.Text.Json.Serialization;

namespace Midas.Application.DTO.NovaPoshta
{
    public record GetCounterpartiesRequest(
        [property: JsonPropertyName("CounterpartyProperty")] string CounterpartyProperty,
        [property: JsonPropertyName("Page")] string Page = "1");

    public record NpCounterpartyItem(string Ref, string Description, string MarketplaceName);
    public record NpSenderDto(string Ref, string Name, string CompanyName);
}
