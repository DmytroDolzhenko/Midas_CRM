using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Midas.Application.Entities.Companies.Commands
{
    public class UpdateCompanyCommand : ICommand<Company>
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? TaxNumber { get; init; }
    }

    public class UpdateCompanyCommandHandler(
        Midas.Application.Common.Interfaces.ICurrentUserService currentUserService,
        IGetQueries<Company, Guid> queries,
        IEntityRepository<Company> repository) : IRequestHandler<UpdateCompanyCommand, Company>
    {
        public async Task<Company> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await queries.GetByIdAsync(request.Id, cancellationToken, q => q.Include(x => x.Members));
            if (company is null)
            {
                throw new Exception($"Company with id {request.Id} not found.");
            }

            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var currentUserMember = company.Members.FirstOrDefault(x => x.UserId == currentUserId);
            if (currentUserMember is null || (currentUserMember.Role is not CompanyRole.Owner and not CompanyRole.Admin))
            {
                throw new UnauthorizedAccessException("Only Owner or Admin can update company.");
            }

            company.UpdateName(request.Name);
            company.UpdateTaxNumber(request.TaxNumber);

            await repository.UpdateAsync(company, cancellationToken);
            return company;
        }
    }
}
