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
    public class AddCompanyMemberCommand : ICommand<CompanyMember>
    {
        public required Guid UserId { get; init; }
        public required CompanyRole Role { get; init; }
    }

    public class AddCompanyMemberCommandHandler(
        ICurrentUserService currentUserService,
        IGetQueries<Company, Guid> companyQueries,
        IEntityRepository<Company> companyRepository) : IRequestHandler<AddCompanyMemberCommand, CompanyMember>
    {
        public async Task<CompanyMember> Handle(AddCompanyMemberCommand request, CancellationToken cancellationToken)
        {
            var companyId = await currentUserService.GetCompanyIdAsync(cancellationToken) ?? throw new UnauthorizedAccessException();
            var company = await companyQueries.GetByIdAsync(companyId, cancellationToken, q => q.Include(x => x.Members));
            if (company is null)
            {
                throw new Exception("Company not found");
            }

            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var currentUserMember = company.Members.FirstOrDefault(x => x.UserId == currentUserId);
            if (currentUserMember is null || (currentUserMember.Role is not CompanyRole.Owner and not CompanyRole.Admin))
            {
                throw new UnauthorizedAccessException("Only Owner or Admin can add company members.");
            }

            company.AddMember(request.UserId, request.Role);
            await companyRepository.UpdateAsync(company, cancellationToken);

            return company.Members.First(x => x.UserId == request.UserId);
        }
    }
}
