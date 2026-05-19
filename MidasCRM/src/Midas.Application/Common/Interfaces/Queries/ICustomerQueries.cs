using Midas.Core.Customers;
using Midas.Core.Enums;
using Midas.Core.ProductVariants;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface ICustomerQueries
    {
        public Task<Customer?> GetCustomerByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
