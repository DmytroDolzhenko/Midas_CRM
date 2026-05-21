using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;

namespace Midas.Application.Entities.CompanyMembers.Commands
{
    public class RemoveCompanyMemberCommand : ICommand<CompanyMember>
    {
        public required Guid UserId { get; init; }
    }

    public class RemoveCompanyMemberCommandHandler(
        ICurrentUserService currentUserService,
        IGetQueries<Company, Guid> companyQueries,
        IEntityRepository<Company> companyRepository) : IRequestHandler<RemoveCompanyMemberCommand, CompanyMember>
    {
        public async Task<CompanyMember> Handle(RemoveCompanyMemberCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var company = await companyQueries.GetByIdAsync(companyId, cancellationToken, q => q.Include(x => x.Members));
            if (company is null)
            {
                throw new Exception("Company not found");
            }

            var member = company.Members.FirstOrDefault(x => x.UserId == request.UserId) ?? throw new Exception("Company member not found");
            company.RemoveMember(request.UserId);

            await companyRepository.UpdateAsync(company, cancellationToken);
            return member;
        }
    }
}
