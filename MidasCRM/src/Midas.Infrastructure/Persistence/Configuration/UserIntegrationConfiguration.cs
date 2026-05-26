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

            builder.HasOne(x => x.Company)
                .WithMany()
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.CreatedByUser)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);



            builder.ToTable("UserIntegrations");
        }
    }
}
