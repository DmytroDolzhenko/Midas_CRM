using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UniqCode)
                .IsRequired();

            builder.HasIndex(x => x.UniqCode)
                .IsUnique();

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.Adress)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.TotalCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.OwnerId)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();


            builder.ToTable("Order");
        }
    }
}
