using MediatR;
using Microsoft.EntityFrameworkCore;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Queries;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;
using Midas.Core.Enums;

namespace Midas.Application.Entities.Companies.Commands
{
    public class AddMemberByEmailCommand : ICommand<Company>
    {
        public required Guid CompanyId { get; init; }
        public required string Email { get; init; }
    }

    public class AddMemberByEmailCommandHandler(
        ICurrentUserService currentUserService,
        IEntityRepository<Company> repository,
        IGetQueries<Company, Guid> getQueries,
        IUserQueries userQueries
        ) : IRequestHandler<AddMemberByEmailCommand, Company>
    {
        public async Task<Company> Handle(AddMemberByEmailCommand request, CancellationToken cancellationToken)
        {
            var company = await getQueries.GetByIdAsync(request.CompanyId, cancellationToken, q => q.Include(x => x.Members));
            if (company is null)
            {
                throw new InvalidOperationException("Company not found");
            }

            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var currentUserMember = company.Members.FirstOrDefault(x => x.UserId == currentUserId);
            if (currentUserMember is null || (currentUserMember.Role is not CompanyRole.Owner and not CompanyRole.Admin))
            {
                throw new UnauthorizedAccessException("Only Owner or Admin can add company members.");
            }

            var user = await userQueries.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
            {
                throw new InvalidOperationException("User not found");
            }

            company.AddMember(user.Id, CompanyRole.Manager);
            await repository.UpdateAsync(company, cancellationToken);
            return company;
        }
    }
}
