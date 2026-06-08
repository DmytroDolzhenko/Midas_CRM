using System.Text.Json.Serialization;

namespace Midas.Application.DTOs.NovaPoshta.Requests
{
    public class NpCreateInternetDocumentProperties
    {
        public string? SenderWarehouseIndex { get; set; }
        public string? RecipientWarehouseIndex { get; set; }
        public string PayerType { get; set; } = "Recipient";
        public string PaymentMethod { get; set; } = "Cash";
        public string DateTime { get; set; } = string.Empty;
        public string CargoType { get; set; } = "Cargo";
        public string ServiceType { get; set; } = string.Empty;
        public string Sender { get; set; } = string.Empty;
        public string CitySender { get; set; } = string.Empty;
        public string SenderAddress { get; set; } = string.Empty;
        public string ContactSender { get; set; } = string.Empty;
        public string SendersPhone { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public string CityRecipient { get; set; } = string.Empty;
        public string RecipientAddress { get; set; } = string.Empty;
        public string ContactRecipient { get; set; } = string.Empty;
        public string RecipientsPhone { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public decimal Cost { get; set; }
        public string Description { get; set; } = string.Empty;
        public string SeatsAmount { get; set; } = "1";
        public decimal? VolumeGeneral { get; set; }
        public List<NpOptionsSeatItem>? OptionsSeat { get; set; }
    }

    public class NpOptionsSeatItem
    {
        [JsonPropertyName("volumetricVolume")]
        public string VolumetricVolume { get; set; } = "0.002";

        [JsonPropertyName("volumetricWidth")]
        public string VolumetricWidth { get; set; } = "20";

        [JsonPropertyName("volumetricLength")]
        public string VolumetricLength { get; set; } = "20";

        [JsonPropertyName("volumetricHeight")]
        public string VolumetricHeight { get; set; } = "5";

        [JsonPropertyName("weight")]
        public string Weight { get; set; } = "0.3";
    }

    public class NpCreateInternetDocumentResult
    {
        public string Ref { get; set; } = string.Empty;
        public string IntDocNumber { get; set; } = string.Empty;
    }
}

