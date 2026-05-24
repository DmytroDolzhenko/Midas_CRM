using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.CustomerAddresses;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
    {
        public void Configure(EntityTypeBuilder<CustomerAddress> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.CustomerId).IsRequired();

            builder.Property(x => x.City)
                .IsRequired()
                .HasMaxLength(100);

            //builder.Property(x => x.PostalCode).IsRequired();

            builder.Property(x => x.PostDepartmentNumber).IsRequired();
            builder.Property(x => x.DeliveryPointType).IsRequired();

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Addresses)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Orders)
                .WithOne(x => x.Address)
                .HasForeignKey(x => x.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("CustomerAddress");
        }
    }
}
