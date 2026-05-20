using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Contacts;
using Midas.Core.NovaPoshta;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration.NovaPoshtaConfigurations
{
    public class NPCitiesConfiguration : IEntityTypeConfiguration<NovaPoshtaCity>
    {
        public void Configure(EntityTypeBuilder<NovaPoshtaCity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.Ref).IsUnique();
            builder.Property(x => x.Description).HasMaxLength(150);
        }
    }
}
