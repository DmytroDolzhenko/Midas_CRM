using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.Property(x => x.UniqCode)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.UniqCode)
                .IsUnique();

            builder.Property(x => x.Color)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Size)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(x => x.StockQuantity)
                .IsRequired();

            builder.Property(x => x.CostPrice)
                .IsRequired();

            builder.Property(x => x.SellPrice)
                .IsRequired();

            builder.ToTable("ProductVariant");
        }
    }
}
