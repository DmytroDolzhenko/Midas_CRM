using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.CompanyMembers;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class CompanyMemberConfiguration : IEntityTypeConfiguration<CompanyMember>
    {
        public void Configure(EntityTypeBuilder<CompanyMember> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(x => x.JoinedAtUtc).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.CompanyId, x.UserId }).IsUnique();

            builder.ToTable("CompanyMember");
        }
    }
}
