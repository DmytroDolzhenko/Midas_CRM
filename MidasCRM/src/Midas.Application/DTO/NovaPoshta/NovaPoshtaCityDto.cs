using System.Text.Json.Serialization;

namespace Midas.Application.DTOs.NovaPoshta
{
    public record GetNPCitiesProperties(
    [property: JsonPropertyName("FindByString")] string FindByString,
    [property: JsonPropertyName("Page")] string Page = "1"
);

    public class NovaPoshtaCityDto
    {
        public string Description { get; set; } = null!; // Назва міста (Київ)
        public string Ref { get; set; } = null!;         //GUID міста для майбутніх методів
        public string AreaDescription { get; set; } = null!; // Область
    }
}
