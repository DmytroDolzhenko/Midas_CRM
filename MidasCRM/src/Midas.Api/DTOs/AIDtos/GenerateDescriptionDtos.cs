namespace Midas.Api.DTOs.AIDtos
{
    public class GenerateDescriptionDto
    {
        public string Type { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string>? Items { get; set; }
    }
}
