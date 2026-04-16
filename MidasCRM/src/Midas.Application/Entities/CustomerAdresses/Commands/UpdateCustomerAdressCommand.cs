using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAdresses;

namespace Midas.Application.Entities.CustomerAdresses.Commands
{
    public class UpdateCustomerAdressCommand : IRequest<CustomerAdress>
    {
        public required int Id { get; init; }
        public required int CustomerId { get; init; }
        public required string City { get; init; }
        public required int PostalCode { get; init; }
        public required int PostDepartmentNumber { get; init; }
    }

    public class UpdateCustomerAdressCommandHandler(
        IGetQueries<CustomerAdress> queries,
        IEntityRepository<CustomerAdress> repository)
        : IRequestHandler<UpdateCustomerAdressCommand, CustomerAdress>
    {
        public async Task<CustomerAdress> Handle(UpdateCustomerAdressCommand request, CancellationToken cancellationToken)
        {
            var customerAdress = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (customerAdress == null)
            {
                throw new Exception($"CustomerAdress with id {request.Id} not found.");
            }

            customerAdress.Update(
                request.CustomerId,
                request.City,
                request.PostalCode,
                request.PostDepartmentNumber);

            await repository.UpdateAsync(customerAdress, cancellationToken);
            return customerAdress;
        }
    }
}
