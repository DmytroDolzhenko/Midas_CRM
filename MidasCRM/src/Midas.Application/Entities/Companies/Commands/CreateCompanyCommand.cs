using MediatR;
using Midas.Application.Common.Interfaces;
using Midas.Application.Common.Interfaces.Repositories;
using Midas.Application.Common.Messaging;
using Midas.Core.Companies;

namespace Midas.Application.Entities.Companies.Commands
{
    public class CreateCompanyCommand : ICommand<Company>
    {
        public required string Name { get; init; }
        public string? TaxNumber { get; init; }
    }

    public class CreateCompanyCommandHandler(
        IEntityRepository<Company> repository,
        ICurrentUserService currentUserService) : IRequestHandler<CreateCompanyCommand, Company>
    {
        public async Task<Company> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var company = Company.Create(request.Name, request.TaxNumber, userId);
            await repository.AddAsync(company, cancellationToken);
            return company;
        }
    }
}
