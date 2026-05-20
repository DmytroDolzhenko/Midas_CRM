using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Contacts;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.PhoneNumber)
                .HasMaxLength(13)
                .IsRequired();

            builder.HasMany(x => x.Customers)
                .WithOne(x => x.Contact)
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Contact");
        }
    }
}
