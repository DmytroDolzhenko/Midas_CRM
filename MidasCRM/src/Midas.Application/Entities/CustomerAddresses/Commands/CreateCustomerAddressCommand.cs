using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAddresses;
using Midas.Application.Common.Messaging;

namespace Midas.Application.Entities.CustomerAddresses.Commands
{
    public class CreateCustomerAddressCommand : ICommand<CustomerAddress>
    {
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
    }

    public class CreateCustomerAdressCommandHandler(
        IEntityRepository<CustomerAddress> repository,
        ICurrentUserService currentUserService)
        : IRequestHandler<CreateCustomerAddressCommand, CustomerAddress>
    {
        public async Task<CustomerAddress> Handle(CreateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var customerAdress = CustomerAddress.Create(
                0,
                request.CustomerId,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber,
                companyId);

            await repository.AddAsync(customerAdress, cancellationToken);
            return customerAdress;
        }
    }
}

