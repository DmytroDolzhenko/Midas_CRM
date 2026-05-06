using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(x => x.Fathername)
                .IsUnique();

            builder.Property(x => x.Role)
                .IsRequired();

            builder.Property(x => x.RegistrationDate)
                .IsRequired();

            builder.Property(x => x.IsApproved)
                .IsRequired();

            builder.ToTable("User");
        }
    }
}
