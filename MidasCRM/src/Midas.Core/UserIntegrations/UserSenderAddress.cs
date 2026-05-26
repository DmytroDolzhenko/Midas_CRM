using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.UserIntegrations
{
    using System;

    namespace Midas.Core.UserIntegrations
    {
        public class UserSenderAddress : IEntity<int>
        {
            public int Id { get; private set; }
            public int UserLogisticProfileId { get; private set; }

            public string CityRef { get; private set; } = null!;         // CitySender
            public string AddressRef { get; private set; } = null!;      // SenderAddress
            public string WarehouseIndex { get; private set; } = null!;  // SenderWarehouseIndex
            public string Description { get; private set; } = null!;     // Назва для відображення менеджера (напр. "Київ, Відділення №1")

            public UserLogisticProfile UserLogisticProfile { get; private set; } = null!;

            private UserSenderAddress() { }

            public static UserSenderAddress Create(
                string cityRef,
                string addressRef,
                string warehouseIndex,
                string description)
            {
                if (string.IsNullOrWhiteSpace(cityRef)) throw new ArgumentException("CityRef is required");
                if (string.IsNullOrWhiteSpace(addressRef)) throw new ArgumentException("AddressRef is required");

                return new UserSenderAddress
                {
                    CityRef = cityRef,
                    AddressRef = addressRef,
                    WarehouseIndex = warehouseIndex ?? string.Empty,
                    Description = description ?? string.Empty
                };
            }

            public void Update(string cityRef, string addressRef, string warehouseIndex, string description)
            {
                if (string.IsNullOrWhiteSpace(cityRef)) throw new ArgumentException("CityRef is required");
                if (string.IsNullOrWhiteSpace(addressRef)) throw new ArgumentException("AddressRef is required");

                CityRef = cityRef;
                AddressRef = addressRef;
                WarehouseIndex = warehouseIndex ?? string.Empty;
                Description = description ?? string.Empty;
            }
        }
    }
}
