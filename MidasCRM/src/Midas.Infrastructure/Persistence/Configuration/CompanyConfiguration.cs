using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Companies;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.TaxNumber)
                .HasMaxLength(50);

            builder.Property(x => x.Balance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreatedAt).IsRequired();

            builder.HasMany(x => x.Members)
                .WithOne(x => x.Company)
                .HasForeignKey(x => x.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Company");
        }
    }
}
