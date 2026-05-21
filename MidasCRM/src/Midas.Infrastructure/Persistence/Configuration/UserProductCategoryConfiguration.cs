using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.UserProductCategories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class UserProductCategoryConfiguration : IEntityTypeConfiguration<UserProductCategory>
    {
        public void Configure(EntityTypeBuilder<UserProductCategory> builder)
        {
            builder.HasKey(uc => new { uc.UserId, uc.ProductCategoryId });

            builder.HasOne(uc => uc.ProductCategory)
                .WithMany(pc => pc.UserCategories)
                .HasForeignKey(uc => uc.ProductCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("UserProductCategory");
        }
    }
}
