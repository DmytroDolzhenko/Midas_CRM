using System.Text.Json.Serialization;

namespace Midas.Application.DTOs.NovaPoshta
{
    public record GetNPWarehousesProperties(
    [property: JsonPropertyName("CityRef")] string CityRef,
    [property: JsonPropertyName("Page")] string Page = "1"
);

    public class NovaPoshtaWarehouseDto
    {
        public string Description { get; set; } = null!; // Назва (Відділення №1)
        public string Ref { get; set; } = null!;         // GUID відділення для ТТН
        public string Number { get; set; } = null!;                  // Номер відділення
        public string ShortAddress { get; set; } = null!;
    }
}
