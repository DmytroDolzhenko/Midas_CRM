using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;

namespace Midas.Application.Entities.Companies.Commands
{
    public class DeleteCompanyCommand : ICommand<Company>
    {
        public required Guid Id { get; init; }
    }

    public class DeleteCompanyCommandHandler(
        IGetQueries<Company, Guid> queries,
        IEntityRepository<Company> repository) : IRequestHandler<DeleteCompanyCommand, Company>
    {
        public async Task<Company> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (company is null)
            {
                throw new Exception($"Company with id {request.Id} not found.");
            }

            company.MarkAsDeleted();
            await repository.UpdateAsync(company, cancellationToken);
            return company;
        }
    }
}
