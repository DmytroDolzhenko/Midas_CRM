using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Customers;

namespace Midas.Application.Entities.Customers.Commands
{
    public class DeleteCustomerCommand : ICommand<Customer>
    {
        public required int Id { get; init; }
    }

    public class DeleteCustomerCommandHandler(
        IGetQueries<Customer, int> queries,
        IEntityRepository<Customer> repository)
        : IRequestHandler<DeleteCustomerCommand, Customer>
    {
        public async Task<Customer> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null)
            {
                throw new Exception($"Customer with id {request.Id} not found.");
            }

            customer.MarkAsDeleted();
            await repository.UpdateAsync(customer, cancellationToken);
            return customer;
        }
    }
}
