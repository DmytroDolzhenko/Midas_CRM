using Midas.Core.Companies;
using Midas.Core.Enums;
using Midas.Core.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midas.Core.CompanyMembers
{
    public class CompanyMember : IEntity<int>, ICompanyOwnedEntity
    {
        public int Id { get; }
        public Guid CompanyId { get; private set; }
        public Company Company { get; private set; } = null!;

        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;

        public CompanyRole Role { get; private set; }
        public DateTime JoinedAtUtc { get; private set; }

        private CompanyMember(int id, Guid companyId, Guid userId, CompanyRole role, DateTime joinedAtUtc)
        {
            Id = id;
            CompanyId = companyId;
            UserId = userId;
            Role = role;
            JoinedAtUtc = joinedAtUtc;
        }
        public static CompanyMember Create(Guid companyId, Guid userId, CompanyRole role)
        {
           var newMember = new CompanyMember(
               0,
               companyId,
               userId,
               role,
               DateTime.UtcNow);
            return newMember;
        }

        public void UpdateRole(CompanyRole newRole)
        {
            Role = newRole;
        }
    }
}
