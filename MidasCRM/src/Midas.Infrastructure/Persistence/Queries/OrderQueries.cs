using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Core.Enums;
using Midas.Core.Orders;
using Midas.Infrastructure.Persistence.Queries.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Infrastructure.Persistence.Queries
{
    public class OrderQueries : IOrderQueries
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public OrderQueries(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        private async Task<IQueryable<Order>> GetFilteredOrdersAsync(CancellationToken cancellationToken)
        {
            var companyId = await _currentUserService.GetCompanyIdAsync(cancellationToken);
            var userId = _currentUserService.UserId;

            return _context.Orders.ApplyCompanyFilter(_context, userId, companyId);
        }
        public async Task<IReadOnlyList<Order?>> GetOrderByCustomerAsync(int customerId, CancellationToken cancellationToken)
        {
            var orderQuery = await GetFilteredOrdersAsync(cancellationToken);

            return await orderQuery
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Order?>> GetOrderByStatusAsync(OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            var orderQuery = await GetFilteredOrdersAsync(cancellationToken);

            return await orderQuery
                .Where(o => o.Status == orderStatus)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .Include(o => o.Customer)
                .Include(o => o.Address)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order?> GetOrderByUniqCodeAsync(string uniqCode, CancellationToken cancellationToken)
        {
            var orderQuery = await GetFilteredOrdersAsync(cancellationToken);

            return await orderQuery
                .Where(o => o.UniqCode == uniqCode)
                .Include(o => o.OrderItems)
                .Include(o => o.Payments)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
