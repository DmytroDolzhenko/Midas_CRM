using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Core.CustomerAdresses;

namespace Midas.Application.Entities.CustomerAdresses.Commands
{
    public class DeleteCustomerAdressCommand : IRequest<CustomerAdress>
    {
        public required int Id { get; init; }
    }

    public class DeleteCustomerAdressCommandHandler(
        IGetQueries<CustomerAdress> queries,
        IEntityRepository<CustomerAdress> repository)
        : IRequestHandler<DeleteCustomerAdressCommand, CustomerAdress>
    {
        public async Task<CustomerAdress> Handle(DeleteCustomerAdressCommand request, CancellationToken cancellationToken)
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
