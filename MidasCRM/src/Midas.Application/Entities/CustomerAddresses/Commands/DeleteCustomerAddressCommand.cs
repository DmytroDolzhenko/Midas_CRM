using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.CustomerAddresses;

namespace Midas.Application.Entities.CustomerAddresses.Commands
{
    public class DeleteCustomerAddressCommand : ICommand<CustomerAddress>
    {
        public required int Id { get; init; }
    }

    public class DeleteCustomerAdressCommandHandler(
        IGetQueries<CustomerAddress, int> queries,
        IEntityRepository<CustomerAddress> repository)
        : IRequestHandler<DeleteCustomerAddressCommand, CustomerAddress>
    {
        public async Task<CustomerAddress> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAdress = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (customerAdress == null)
            {
                throw new Exception($"CustomerAdress with id {request.Id} not found.");
            }

            customerAdress.Delete();
            await repository.DeleteAsync(customerAdress, cancellationToken);
            return customerAdress;
        }
    }
}
