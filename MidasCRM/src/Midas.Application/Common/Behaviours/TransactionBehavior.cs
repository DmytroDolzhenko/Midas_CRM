using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Midas.Application.Common.Behaviours
{
    public class TransactionBehavior<TRequest, TResponse>
        (IApplicationDbContext applicationDbContext)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand<TResponse>
    {
        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
            )
        {
            using var transaction = await applicationDbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var responce = await next();

                await applicationDbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return responce;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
