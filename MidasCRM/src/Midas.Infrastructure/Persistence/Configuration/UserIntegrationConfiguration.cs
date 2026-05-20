using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.UserIntegrations;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class UserIntegrationConfiguration : IEntityTypeConfiguration<UserIntegration>
    {
        public void Configure(EntityTypeBuilder<UserIntegration> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Provider)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.EncryptedAccessToken)
                .IsRequired(false);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasIndex(x => new { x.UserId, x.Provider })
                .IsUnique();

            builder.HasOne(ui => ui.LogisticProfile)
                .WithOne(lp => lp.UserIntegration)
                .HasForeignKey<UserLogisticProfile>(lp => lp.UserIntegrationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("UserIntegrations");
        }
    }
}
