using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.NovaPoshta
{
    public class NovaPoshtaWarehouse : IEntity<int>
    {
        public int Id { get; private set; }
        public string Ref { get; private set; } = null!;         // GUID складу від НП
        public string CityRef { get; private set; } = null!;     // Зв'язок з містом через текстовий Ref НП
        public string Description { get; private set; } = null!; // Назва (напр. "Відділення №1: вул. Пирогівський шлях, 135")
        public string Number { get; private set; } = null!;                  // Номер відділення
        public string WarehouseIndex { get; private set; } = null!; // Цифрова адреса (напр. "32/1a")
        public string TypeOfWarehouse { get; private set; } = null!; // Поштомат, Відділення, Вантажне відділення

        private NovaPoshtaWarehouse() { }

        public static NovaPoshtaWarehouse Create(string @ref, string cityRef, string description, string number, string index, string type)
        {
            return new NovaPoshtaWarehouse
            {
                Ref = @ref,
                CityRef = cityRef,
                Description = description,
                Number = number,
                WarehouseIndex = index,
                TypeOfWarehouse = type
            };
        }

        public void Update(string description, string number, string index, string type)
        {
            Description = description;
            Number = number;
            WarehouseIndex = index;
            TypeOfWarehouse = type;
        }
    }
}
