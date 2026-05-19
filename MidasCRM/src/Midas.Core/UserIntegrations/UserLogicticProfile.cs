using Midas.Core.UserIntegrations.Midas.Core.UserIntegrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.UserIntegrations
{
    public class UserLogisticProfile : IEntity<int>
    {
        public int Id { get; private set; }
        public int UserIntegrationId { get; private set; }

        public string SenderRef { get; private set; } = null!;
        public string ContactSenderRef { get; private set; } = null!;
        public string SendersPhone { get; private set; } = null!;

        public UserIntegration UserIntegration { get; private set; } = null!;

        private readonly List<UserSenderAddress> _senderAddresses = new();
        public IReadOnlyCollection<UserSenderAddress> SenderAddresses => _senderAddresses.AsReadOnly();

        private UserLogisticProfile() { }

        public static UserLogisticProfile Create(
            int userIntegrationId,
            string senderRef,
            string contactSenderRef,
            string sendersPhone)
        {
            if (string.IsNullOrWhiteSpace(senderRef)) throw new ArgumentException("SenderRef is required");
            if (string.IsNullOrWhiteSpace(contactSenderRef)) throw new ArgumentException("ContactSenderRef is required");
            if (string.IsNullOrWhiteSpace(sendersPhone)) throw new ArgumentException("SendersPhone is required");

            return new UserLogisticProfile
            {
                UserIntegrationId = userIntegrationId,
                SenderRef = senderRef,
                ContactSenderRef = contactSenderRef,
                SendersPhone = CleanPhoneNumber(sendersPhone)
            };
        }
        public static UserLogisticProfile CreateEmpty(int userIntegrationId)
        {
            return new UserLogisticProfile
            {
                UserIntegrationId = userIntegrationId,
                SenderRef = string.Empty,
                ContactSenderRef = string.Empty,
                SendersPhone = string.Empty
            };
        }
        public void AddAddress(UserSenderAddress address)
        {
            _senderAddresses.Add(address);
        }

        private static string CleanPhoneNumber(string phone)
        {
            var cleaned = new string(phone.Where(char.IsDigit).ToArray());
            return cleaned.StartsWith("38") ? cleaned : $"38{cleaned}";
        }

        public void Update(string senderRef, string contactSenderRef, string sendersPhone)
        {
            if (string.IsNullOrWhiteSpace(senderRef)) throw new ArgumentException("SenderRef is required");
            if (string.IsNullOrWhiteSpace(contactSenderRef)) throw new ArgumentException("ContactSenderRef is required");
            if (string.IsNullOrWhiteSpace(sendersPhone)) throw new ArgumentException("SendersPhone is required");

            SenderRef = senderRef;
            ContactSenderRef = contactSenderRef;
            SendersPhone = CleanPhoneNumber(sendersPhone);
        }
    }
}
