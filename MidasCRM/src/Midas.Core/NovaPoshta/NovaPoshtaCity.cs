using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.NovaPoshta
{
    public class NovaPoshtaCity : IEntity<int>
    {
        public int Id { get; private set; }
        public string Ref { get; private set; } = null!;          // GUID від НП (напр. "db5c888c-...")
        public string Description { get; private set; } = null!;  // Назва українською (напр. "Київ")
        public string SettlementTypeDescription { get; private set; } = null!; // "м.", "смт.", "с."
        public string AreaDescription { get; private set; } = null!; // Область (напр. "Київська")

        // Створюємо зручне обчислювальне поле для випадаючих списків (Пошуку)
        public string SearchName => $"{SettlementTypeDescription} {Description} ({AreaDescription} обл.)";

        private NovaPoshtaCity() { }

        public static NovaPoshtaCity Create(string @ref, string description, string settlementType, string area)
        {
            return new NovaPoshtaCity
            {
                Ref = @ref,
                Description = description,
                SettlementTypeDescription = settlementType,
                AreaDescription = area
            };
        }

        public void Update(string description, string settlementType, string area)
        {
            Description = description;
            SettlementTypeDescription = settlementType;
            AreaDescription = area;
        }
    }
}
