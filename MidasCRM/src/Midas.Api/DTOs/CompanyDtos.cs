using Midas.Core.Companies;
using Midas.Core.CompanyMembers;
using Midas.Core.Enums;

namespace Api.Dtos
{
    public record CompanyDto(Guid Id, string Name, string? TaxNumber, DateTime CreatedAt, bool IsDeleted, IReadOnlyCollection<CompanyMemberDto> Members)
    {
        public static CompanyDto FromDomain(Company company)
            => new(company.Id, company.Name, company.TaxNumber, company.CreatedAt, company.IsDeleted, company.Members.Select(CompanyMemberDto.FromDomain).ToList());
    }

    public record CompanyMemberDto(int Id, Guid CompanyId, Guid UserId, CompanyRole Role, DateTime JoinedAtUtc)
    {
        public static CompanyMemberDto FromDomain(CompanyMember member)
            => new(member.Id, member.CompanyId, member.UserId, member.Role, member.JoinedAtUtc);
    }

    public record CreateCompanyDto(string Name, string? TaxNumber);
    public record UpdateCompanyDto(string Name, string? TaxNumber);
    public record AddCompanyMemberDto(Guid UserId, CompanyRole Role);
    public record UpdateCompanyMemberRoleDto(CompanyRole Role);
}
