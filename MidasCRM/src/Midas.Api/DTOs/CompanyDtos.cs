using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using Midas.Core.Enums;

namespace Api.Dtos
{
    public record CompanyDto(Guid Id, string Name, string? TaxNumber, decimal Balance, DateTime CreatedAt, bool IsDeleted, IReadOnlyCollection<CompanyMemberDto> Members)
    {
        public static CompanyDto FromDomain(Company company)
            => new(company.Id, company.Name, company.TaxNumber, company.Balance, company.CreatedAt, company.IsDeleted, company.Members.Select(CompanyMemberDto.FromDomain).ToList());
    }

    public record CompanyMemberUserDto(Guid Id, string Name, string Surname, string Email);

    public record CompanyMemberDto(int Id, Guid CompanyId, Guid UserId, CompanyRole Role, DateTime JoinedAtUtc, CompanyMemberUserDto? User)
    {
        public static CompanyMemberDto FromDomain(CompanyMember member)
            => new(
                member.Id,
                member.CompanyId,
                member.UserId,
                member.Role,
                member.JoinedAtUtc,
                member.User is null
                    ? null
                    : new CompanyMemberUserDto(
                        member.User.Id,
                        member.User.Name,
                        member.User.Surname,
                        member.User.Email ?? string.Empty));
    }

    public record CompanyBalanceDto(Guid CompanyId, decimal Balance);

    public record CreateCompanyDto(string Name, string? TaxNumber);
    public record UpdateCompanyDto(string Name, string? TaxNumber);
    public record AddCompanyMemberDto(Guid UserId, CompanyRole Role);
    public record UpdateCompanyMemberRoleDto(CompanyRole Role);
    public record AddMemberByEmailDto(Guid CompanyId, string Email);
}
