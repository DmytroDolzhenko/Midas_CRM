using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Enums;
using Midas.Core.FinancialOperations;
using Midas.Core.Orders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Application.Entities.Orders.Commands
{
    public class UpdateOrderStatusCommand : ICommand<Order>
    {
        public required Guid OrderId { get; init; }
        public required OrderStatus Status { get; init; }
    }
    public class UpdateOrderStatusCommandHandler(
        IGetQueries<Order, Guid> getQueries,
        IEntityRepository<Order> entityRepository,
        ICurrentUserService currentUserService,
        IApplicationDbContext context) : IRequestHandler<UpdateOrderStatusCommand, Order>
    {
        public async Task<Order> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var order = await getQueries.GetByIdAsync(request.OrderId, cancellationToken);

            if (order == null)
            {
                throw new Exception($"Order with ID {request.OrderId} not found.");
            }

            var oldStatus = order.Status;
            order.UpdateStatus(request.Status);

            if (oldStatus != OrderStatus.Received && request.Status == OrderStatus.Received)
            {
                var operationExists = await context.FinancialOperations
                    .AnyAsync(x =>
                        x.OrderId == order.Id &&
                        x.OperationType == FinancialOperationType.Accrual &&
                        x.Category == FinancialOperationCategory.Sale &&
                        !x.IsDeleted,
                        cancellationToken);

                if (!operationExists)
                {
                    var company = await context.Companies
                        .FirstOrDefaultAsync(x => x.Id == order.CompanyId, cancellationToken)
                        ?? throw new Exception($"Company with id {order.CompanyId} not found.");

                    var operation = FinancialOperation.Create(
                        order.CompanyId,
                        FinancialOperationType.Accrual,
                        FinancialOperationCategory.Sale,
                        order.TotalCost,
                        $"Автоматичне нарахування за замовлення {order.UniqCode}",
                        order.Id,
                        null);

                    company.ApplyFinancialOperation(FinancialOperationType.Accrual, order.TotalCost);
                    await context.FinancialOperations.AddAsync(operation, cancellationToken);
                }
            }

            await entityRepository.UpdateAsync(order, cancellationToken);
            return order;
        }
    }
}
