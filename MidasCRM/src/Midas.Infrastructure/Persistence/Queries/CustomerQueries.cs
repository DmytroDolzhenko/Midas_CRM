using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Customers;
using Midas.Core.Enums;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class CustomerQueries : ICustomerQueries
    {
        private readonly ApplicationDbContext _context;
        public CustomerQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return _context.Customers
                .Include(c => c.Contact)
                .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
        }
    }
}
