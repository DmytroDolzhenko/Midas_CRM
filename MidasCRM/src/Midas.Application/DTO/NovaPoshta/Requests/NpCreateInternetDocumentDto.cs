namespace Midas.Application.DTOs.NovaPoshta.Requests
{
    public class NpCreateInternetDocumentProperties
    {
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
    }

    public class NpCreateInternetDocumentResult
    {
        public string Ref { get; set; } = string.Empty;
        public string IntDocNumber { get; set; } = string.Empty;
    }
}
