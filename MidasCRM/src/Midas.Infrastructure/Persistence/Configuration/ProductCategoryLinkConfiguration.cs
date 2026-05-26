using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Products;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    namespace Midas.Infrastructure.Persistence.Configuration
    {
        public class ProductCategoryLinkConfiguration : IEntityTypeConfiguration<ProductCategoryLink>
        {
            public void Configure(EntityTypeBuilder<ProductCategoryLink> builder)
            {
                builder.HasKey(x => new { x.ProductId, x.CategoryId });

                builder.HasOne(x => x.Product)
                    .WithMany(p => p.ProductCategories)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.Category)
                    .WithMany()
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.ToTable("ProductCategoryLink");
            }
        }
    }
}
