using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using Midas.Core.Enums;

namespace Midas.Application.Entities.CompanyMembers.Commands
{
    public class UpdateCompanyMemberRoleCommand : ICommand<CompanyMember>
    {
        public required Guid UserId { get; init; }
        public required CompanyRole Role { get; init; }
    }

    public class UpdateCompanyMemberRoleCommandHandler(
        ICurrentUserService currentUserService,
        IGetQueries<Company, Guid> companyQueries,
        IEntityRepository<Company> companyRepository) : IRequestHandler<UpdateCompanyMemberRoleCommand, CompanyMember>
    {
        public async Task<CompanyMember> Handle(UpdateCompanyMemberRoleCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var company = await companyQueries.GetByIdAsync(companyId, cancellationToken, q => q.Include(x => x.Members));
            if (company is null)
            {
                throw new Exception("Company not found");
            }

            var member = company.Members.FirstOrDefault(x => x.UserId == request.UserId) ?? throw new Exception("Company member not found");
            member.UpdateRole(request.Role);

            await companyRepository.UpdateAsync(company, cancellationToken);
            return member;
        }
    }
}
