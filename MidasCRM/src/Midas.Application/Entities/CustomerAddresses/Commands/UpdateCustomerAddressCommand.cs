using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAddresses;

namespace Midas.Application.Entities.CustomerAddresses.Commands
{
    public class UpdateCustomerAddressCommand : IRequest<CustomerAddress>
    {
        public required int Id { get; init; }
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
    }

    public class UpdateCustomerAddressCommandHandler(
        IGetQueries<CustomerAddress, int> queries,
        IEntityRepository<CustomerAddress> repository)
        : IRequestHandler<UpdateCustomerAddressCommand, CustomerAddress>
    {
        public async Task<CustomerAddress> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var customerAddress = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (customerAddress == null)
            {
                throw new Exception($"CustomerAddress with id {request.Id} not found.");
            }

            customerAddress.Update(
                request.CustomerId,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber);

            await repository.UpdateAsync(customerAddress, cancellationToken);
            return customerAddress;
        }
    }
}
