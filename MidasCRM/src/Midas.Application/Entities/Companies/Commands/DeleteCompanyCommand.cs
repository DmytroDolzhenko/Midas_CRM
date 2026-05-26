using MediatR;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Midas.Application.Entities.Companies.Commands
{
    public class DeleteCompanyCommand : ICommand<Company>
    {
        public required Guid Id { get; init; }
    }

    public class DeleteCompanyCommandHandler(
        Midas.Application.Common.Interfaces.ICurrentUserService currentUserService,
        IGetQueries<Company, Guid> queries,
        IEntityRepository<Company> repository) : IRequestHandler<DeleteCompanyCommand, Company>
    {
        public async Task<Company> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
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
                throw new UnauthorizedAccessException("Only Owner or Admin can delete company.");
            }

            company.MarkAsDeleted();
            await repository.UpdateAsync(company, cancellationToken);
            return company;
        }
    }
}
