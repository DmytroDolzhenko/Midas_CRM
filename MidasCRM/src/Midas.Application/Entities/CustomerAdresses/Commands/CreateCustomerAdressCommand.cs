using MediatR;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAdresses;

namespace Midas.Application.Entities.CustomerAdresses.Commands
{
    public class CreateCustomerAdressCommand : IRequest<CustomerAdress>
    {
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
    }

    public class CreateCustomerAdressCommandHandler(IEntityRepository<CustomerAdress> repository)
        : IRequestHandler<CreateCustomerAdressCommand, CustomerAdress>
    {
        public async Task<CustomerAdress> Handle(CreateCustomerAdressCommand request, CancellationToken cancellationToken)
        {
            var customerAdress = CustomerAdress.Create(
                0,
                request.CustomerId,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber);

            await repository.AddAsync(customerAdress, cancellationToken);
            return customerAdress;
        }
    }
}
