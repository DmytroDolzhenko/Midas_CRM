using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Enums;
using Midas.Core.Orders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly ApplicationDbContext _context;
        public OrderQueries(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<Order?>> GetOrderByCustomerAsync(int customerId, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order?>> GetOrderByStatusAsync(OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.Status == orderStatus)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderByUniqCodeAsync(string uniqCode, CancellationToken cancellationToken)
        {
            return await _context.Orders
                .Where(o => o.UniqCode == uniqCode)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
