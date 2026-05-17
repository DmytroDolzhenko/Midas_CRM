using Midas.Core.Enums;
using Midas.Core.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Common.Interfaces.Queries
{
    public interface IOrderQueries
    {
        public Task<IReadOnlyList<Order?>> GetOrderByStatusAsync(OrderStatus orderStatus, CancellationToken cancellationToken);
        public Task<IReadOnlyList<Order?>> GetOrderByCustomerAsync(int customerId, CancellationToken cancellationToken);
        public Task<Order?> GetOrderByUniqCodeAsync(string uniqCode, CancellationToken cancellationToken);

    }
}
