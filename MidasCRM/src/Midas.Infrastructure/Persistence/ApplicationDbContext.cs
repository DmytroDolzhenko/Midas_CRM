using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Core.Contacts;
using Midas.Core.CustomerAddresses;
using Midas.Core.Customers;
using Midas.Core.OrderItems;
using Midas.Core.Orders;
using Midas.Core.OrderSources;
using Midas.Core.Payments;
using Midas.Core.ProductCategories;
using Midas.Core.ProductImages;
using Midas.Core.Products;
using Midas.Core.ProductVariants;
using Midas.Core.UserIntegrations;
using Midas.Core.Users;
using Midas.Core.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options), IApplicationDbContext
    {
        public DbSet<User> Users { get; init; }
        public DbSet<Warehouse> Warehouses { get; init; }
        public DbSet<ProductVariant> ProductVariants { get; init; }
        public DbSet<Product> Products { get; init; }
        public DbSet<ProductCategory> ProductCategories { get; init; }
        public DbSet<Payment> Payments { get; init; }
        public DbSet<Order> Orders { get; init; }
        public DbSet<OrderSource> OrderSources { get; init; }
        public DbSet<OrderItem> OrderItems { get; init; }
        public DbSet<Customer> Customers { get; init; }
        public DbSet<CustomerAddress> CustomerAddresses { get; init; }
        public DbSet<Contact> Contacts { get; init; }
        public DbSet<ProductImage> ProductImages { get; init; }
        public DbSet<UserIntegration> UserIntegrations { get; init; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
