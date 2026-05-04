using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;
using Midas.Core.Customers;

namespace Midas.Application.Entities.Customers.Commands
{
    public class UpdateCustomerCommand : ICommand<Customer>
    {
        public required int Id { get; init; }
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string ContactValue { get; init; }
        public required int Email { get; init; }
    }

    public class UpdateCustomerCommandHandler(
        IGetQueries<Customer, int> queries,
        IEntityRepository<Customer> repository)
        : IRequestHandler<UpdateCustomerCommand, Customer>
    {
        public async Task<Customer> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (customer == null)
            {
                throw new Exception($"Customer with id {request.Id} not found.");
            }

            var contact = Contact.Create(request.ContactValue);
            customer.Update(
                request.Name,
                request.Surname,
                contact,
                request.Email);

            await repository.UpdateAsync(customer, cancellationToken);
            return customer;
        }
    }
}
