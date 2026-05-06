using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.CustomerAdresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class CustomerAdressConfiguration : IEntityTypeConfiguration<CustomerAdress>
    {
        public void Configure(EntityTypeBuilder<CustomerAdress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.PostalCode)
                .IsRequired();

            builder.Property(x => x.PostDepartmentNumber)
                .IsRequired ()
                .HasMaxLength(10);

            builder.ToTable("CustomerAdress");
        }
    }
}
