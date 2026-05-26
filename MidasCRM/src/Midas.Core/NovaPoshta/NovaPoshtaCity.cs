using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.NovaPoshta
{
    public class NovaPoshtaCity : IEntity<int>
    {
        public int Id { get; private set; }
        public string Ref { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public string SettlementTypeDescription { get; private set; } = null!;
        public string AreaDescription { get; private set; } = null!;

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
