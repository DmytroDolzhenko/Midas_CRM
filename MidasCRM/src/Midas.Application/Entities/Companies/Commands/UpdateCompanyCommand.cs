using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;

namespace Midas.Application.Entities.Companies.Commands
{
    public class UpdateCompanyCommand : ICommand<Company>
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? TaxNumber { get; init; }
    }

    public class UpdateCompanyCommandHandler(
        IGetQueries<Company, Guid> queries,
        IEntityRepository<Company> repository) : IRequestHandler<UpdateCompanyCommand, Company>
    {
        public async Task<Company> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await queries.GetByIdAsync(request.Id, cancellationToken);
            if (company is null)
            {
                throw new Exception($"Company with id {request.Id} not found.");
            }

            company.UpdateName(request.Name);
            company.UpdateTaxNumber(request.TaxNumber);

            await repository.UpdateAsync(company, cancellationToken);
            return company;
        }
    }
}
