using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Contacts;
using Midas.Core.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration.NovaPoshtaConfigurations
{
    public class NPWarehousesConfiguration : IEntityTypeConfiguration<NovaPoshtaWarehouse>
    {
        public void Configure(EntityTypeBuilder<NovaPoshtaWarehouse> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Ref).IsUnique();
            builder.HasIndex(x => x.CityRef);
            builder.Property(x => x.Description).HasMaxLength(500);
        }
    }
}
