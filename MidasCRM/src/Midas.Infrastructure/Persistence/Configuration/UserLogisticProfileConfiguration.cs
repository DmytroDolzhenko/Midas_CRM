using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.UserIntegrations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class UserLogisticProfileConfiguration : IEntityTypeConfiguration<UserLogisticProfile>
    {
        public void Configure(EntityTypeBuilder<UserLogisticProfile> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.SenderRef).IsRequired().HasMaxLength(36);
            builder.Property(x => x.ContactSenderRef).IsRequired().HasMaxLength(36);
            builder.Property(x => x.SendersPhone).IsRequired().HasMaxLength(20);

            builder.HasMany(lp => lp.SenderAddresses)
                .WithOne(sa => sa.UserLogisticProfile)
                .HasForeignKey(sa => sa.UserLogisticProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(lp => lp.SenderAddresses)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable("UserLogisticProfiles");
        }
    }
}
