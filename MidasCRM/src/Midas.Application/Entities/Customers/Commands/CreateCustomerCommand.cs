using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Contacts;
using Midas.Core.Customers;

namespace Midas.Application.Entities.Customers.Commands
{
    public class CreateCustomerCommand : ICommand<Customer>
    {
        public required string Name { get; init; }
        public required string Surname { get; init; }
        public required string ContactValue { get; init; }
        public required string Email { get; init; }
    }

    public class CreateCustomerCommandHandler(
        IEntityRepository<Customer> repository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateCustomerCommand, Customer>
    {
        public async Task<Customer> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var contact = Contact.Create(request.ContactValue, currentUserId);
            var customer = Customer.Create(
                request.Name,
                request.Surname,
                contact,
                request.Email,
                currentUserId);

            await repository.AddAsync(customer, cancellationToken);
            return customer;
        }
    }
}
